﻿using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectorInterface.GalvoInterface
{
    static class AnimationManager
    {

        // List of all images which are going to be displayed one after another
        public static List<VectorizedImage> Images;

        // Seperate thread for sending the image data to the arduino
        static Thread SendImgThread;
        static bool Running = false;
        static bool StopCurrentImg = false;

        public delegate void IndexChangedHandler(int oldVal, int newVal);
        public static event IndexChangedHandler? OnImgIndexChanged;
        public static int CurrentImgIndex
        {
            get => _CurrentImgIndex;
            set
            {
                OnImgIndexChanged?.Invoke(_CurrentImgIndex, value);
                _CurrentImgIndex = value;
            }
        }
        static int _CurrentImgIndex;

        static AnimationManager()
        {
            SendImgThread = null!;

            Images = new List<VectorizedImage>();
        }

        // Starts the thread if it isn't already running and some images were loaded in
        public static void Start()
        { 
            if (Running || Images.Count == 0 || !SerialManager.IsConnected)
                return;

            // Triggers IndexChanged Event so that is gets a RenderedItemBorder
            CurrentImgIndex = CurrentImgIndex;

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

        // Skips the current animation
        public static void SkipAnimation()
        {
            if (CurrentImgIndex < Images.Count - 1)
            {
                CurrentImgIndex++;
                if (Running)
                    StopCurrentImg = true;
            }
        }

        // Reverts to the last animation
        public static void RevertAnimation()
        {
            if (Images.Count > 0 && CurrentImgIndex > 0)
            {
                CurrentImgIndex--;
                if (Running)
                    StopCurrentImg = true;
            }
        }

        // Loads all the .ild files from the selected folder into the Images - list
        public static void LoadImagesFromFolder(string path)
        {
            CurrentImgIndex = 0;

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            lock (Images)
            {
                FileInfo[] files = dirInfo.GetFiles("*.ild")
                    .OrderBy(f => f.Name)
                    .ToArray();

                FileInfo currentFile;
                // Reads 100 files max
                for (int i = 0; i < files.Length && i < 100; i++)
                {
                    currentFile = files[i];
                    ILDParser.LoadFromPath(currentFile.FullName, currentFile.Name, ref Images);
                }
            }

            Start();
        }

        public static void AddImage(VectorizedImage img)
        {
            lock (Images)
                Images.Add(img);
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
                        for (int replayIndex = 0; replayIndex < currentFrame.ReplayCount; replayIndex++)
                        {
                            // For measuring how long it takes to send a frame
                            stopwatch.Restart();

                            // Sending the frame
                            SerialManager.SendFrame(currentFrame);

                            stopwatch.Stop();

                            // If the frame was sent faster than 1/FPS seconds the thread will sleep the rest of the time
                            if (stopwatch.ElapsedMilliseconds < Settings.INV_SCAN_FPS_MS)
                                Thread.Sleep(Settings.INV_SCAN_FPS_MS - (int)stopwatch.ElapsedMilliseconds);
                        }
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