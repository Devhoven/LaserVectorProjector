using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ProjectorInterface.GalvoInterface
{
    static class SerialManager
    {
        // Defines the baud rate at which the pc is going to communicate with the arduino
        const int BAUD_RATE = 2_000_000;
        // Defines the size of the buffer in bytes
        // The first two bytes are for the position (between 0 and 4100, normally) 
        // The fifth byte is for the delay (in microseconds) and the last bit of it is for if the laser is turned on for this line
        const int BUFFER_SIZE = 4;

        // Port object through which the communication is going to be held
        static SerialPort Port;

        // List of all images which are going to be displayed one after another
        public static List<VectorizedImage> Images;

        // Seperate thread for sending the image data to the arduino
        static Thread SendImgThread;
        static bool Running = false;
        static bool StopCurrentImg = false;

        public delegate void IndexChangedHandler(int oldVal, int newVal);
        public static event IndexChangedHandler? OnImgIndexChanged;
        static int CurrentImgIndex
        {
            get => _CurrentImgIndex;
            set
            {
                OnImgIndexChanged?.Invoke(_CurrentImgIndex, value);
                _CurrentImgIndex = value;
            }
        }
        static int _CurrentImgIndex;
        static byte[] Buffer;

        static SerialManager()
        {
            Port = null!;
            SendImgThread = null!;

            Images = new List<VectorizedImage>();
            Buffer = new byte[BUFFER_SIZE];
        }

        // Initializes the port 
        public static void Initialize(string portName)
        {
            if (Port != null)
                Port.Close();

            Port = new SerialPort(portName, BAUD_RATE);

            // If a port gets selected that cannot be opened, this would throw an exception
            try
            {
                Port.Open();
            }
            catch { }
        }

        // Starts the thread if it isn't already running and some images were loaded in
        public static void Start()
        {
            if (Running || Images.Count == 0 || Port == null || !Port.IsOpen)
                return;

            Running = true;
            SendImgThread = new Thread(new ThreadStart(SendImgLoop));
            SendImgThread.Start();
        }

        // Stops the thread if it's running and joins it to the main thread
        public static void Stop()
        {
            if (Running)
            {
                Running = false;
                StopCurrentImg = true;
                SendImgThread.Join();
            }
        }

        // Loads all the .ild files from the selected folder into the Images - list
        public static void LoadImagesFromFolder(string path)
        {
            CurrentImgIndex = 0;

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            lock (Images)
            {
                foreach (var dir in dirInfo.GetFiles())
                {
                    if (dir.Name.EndsWith(".ild"))
                        ILDParser.LoadFromPath(dir.FullName, ref Images);
                }
            }

            Start();
        }

        public static void AddImage(VectorizedImage img)
        {
            lock (Images)
                Images.Add(img);
        }

        // Removes the given image from the list and sets the the CurrenImgIndex to 0, in order to prevent errors
        public static void RemoveImg(VectorizedImage img)
        {
            CurrentImgIndex = 0;
            lock (Images)
                Images.Remove(img);
            StopCurrentImg = true;
        }

        // Clears all of the loaded images
        public static void ClearImages()
        {
            CurrentImgIndex = 0;
            lock (Images)
                Images.Clear();
            StopCurrentImg = true;
        }

        static void SendImgLoop()
        {
            // This stopwatch ensures, that 24 frames are diplayed and not any more
            Stopwatch stopwatch = new Stopwatch();
            VectorizedImage currentImg;
            VectorizedFrame currentFrame;
            Line currentLine;
            // Contain the coordinates which are going to be sent to the arduino
            short correctedX, correctedY;
            while (Running)
            {
                // Setting it to false here, so not every frame after a delete call gets skipped
                StopCurrentImg = false;

                currentImg = Images[CurrentImgIndex];
                // Locking the image, since this method runs in a different thread and delete functionality is going to be implemented
                lock (currentImg)
                {
                    for (int i = 0; i < currentImg.FrameCount; i++)
                    {
                        currentFrame = currentImg[i];
                        stopwatch.Restart();
                        // Looping through all of the lines contained in the current image
                        for (int j = 0; j < currentFrame.LineCount; j++)
                        {
                            currentLine = currentFrame.Lines[j];

                            // Maps the coordinates from 0 to 4096 to the correct image section
                            correctedX = (short)((currentLine.X * Settings.IMG_SECTION) + Settings.IMG_OFFSET);
                            correctedY = (short)((currentLine.Y * Settings.IMG_SECTION) + Settings.IMG_OFFSET);

                            // Writing the x and y coordinates into the buffer 
                            Buffer[0] = (byte)correctedX;
                            Buffer[1] = (byte)(correctedX >> 8);
                            Buffer[2] = (byte)correctedY;
                            Buffer[3] = (byte)(correctedY >> 8);

                            if (currentLine.On)
                                Buffer[3] |= 0x80;

                            // Sending the data
                            Port.Write(Buffer, 0, BUFFER_SIZE);
                        }

                        // If the was sent faster than ~41ms (1/24s) the thread will sleep the rest of the time
                        if (stopwatch.ElapsedMilliseconds < 41)
                            Thread.Sleep(41 - (int)stopwatch.ElapsedMilliseconds);

                        // If this bool is set, the current animation got deleted or swapped
                        if (StopCurrentImg)
                            break;
                    }
                }
                // If the list of images got cleared, this will ensure that the thread waits for the list to be filled again
                while (Images.Count == 0) ;
                // Moving on to the next image, or looping to the beginning
                if (!StopCurrentImg)
                    CurrentImgIndex = (CurrentImgIndex + 1) % Images.Count;
            }
        }
    }
}