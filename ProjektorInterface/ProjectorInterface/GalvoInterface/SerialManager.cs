using ProjectorInterface.Helper;
using System.Collections.Generic;
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
        static List<VectorizedImage> Images;

        // Seperate thread for sending the image data to the arduino
        static Thread SendImgThread;
        static bool Running = false;

        static int CurrentImgIndex;
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

            if (Port == null)
                Port = new SerialPort(portName, BAUD_RATE);

            Port.Open();
        }

        // Starts the thread if it isn't already running and some images were loaded in
        public static void Start()
        {
            if (Running || Images.Count == 0)
                return;

            Running = true;
            CurrentImgIndex = 0;
            SendImgThread = new Thread(new ThreadStart(SendImgLoop));
            SendImgThread.Start();
        }

        // Stops the thread if it's running and joins it to the main thread
        public static void Stop()
        {
            if (Running)
            {
                Running = false;
                SendImgThread.Join();
            }
        }

        // Loads all the .ild files from the selected folder into the Images - list
        public static void LoadImagesFromFolder(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (var dir in dirInfo.GetFiles())
            {
                if (dir.Name.EndsWith(".ild"))
                    ILDParser.LoadFromPath(dir.FullName, ref Images);
            }
        }

        // Removes the given image from the list and sets the the CurrenImgIndex to 0, in order to prevent errors
        public static void RemoveImg(VectorizedImage img)
        {
            CurrentImgIndex = 0;
            Images.Remove(img);
        }

        // Clears all of the loaded images
        public static void ClearImages()
        {
            CurrentImgIndex = 0;
            Images.Clear();
        }

        static void SendImgLoop()
        {
            VectorizedImage currentImg;
            Line currentLine;
            while (Running)
            {
                currentImg = Images[CurrentImgIndex];
                // Locking the image, since this method runs in a different thread and delete functionality is going to be implemented
                lock (currentImg)
                {
                    // Looping through all of the lines contained in the current image
                    for (int i = 0; i < currentImg.Lines.Length; i++)
                    {
                        currentLine = currentImg.Lines[i];

                        // Writing the x and y coordinates into the buffer 
                        Buffer[0] = (byte)currentLine.X;
                        Buffer[1] = (byte)(currentLine.X >> 8);
                        Buffer[2] = (byte)currentLine.Y;
                        Buffer[3] = (byte)(currentLine.Y >> 8);

                        // Sending the data
                        Port.Write(Buffer, 0, BUFFER_SIZE);
                    }
                }
                // Moving on to the next image, or looping to the beginning
                CurrentImgIndex = (CurrentImgIndex + 1) % Images.Count;
            }
        }
    }
}