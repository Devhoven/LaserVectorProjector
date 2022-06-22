using ProjectorInterface.GalvoInterface;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectorInterface.GalvoInterface
{
    static class AnimationManager
    {
        public enum Source
        {
            UserImage,
            AnimationGallery
        }

        // Seperate thread for sending the image data to the arduino
        static Thread SendImgThread;
        static bool StopCurrentImg = false;

        public delegate void SourceChangedHandler(Source newSource);
        public static event SourceChangedHandler? OnSourceChanged;

        static Dictionary<Source, Animation> AnimationSources;

        static Animation CurrentAnimation;
        static Source CurrentSource;
        
        static AnimationManager()
        {
            SendImgThread = null!;
            CurrentAnimation = null!;

            AnimationSources = new Dictionary<Source, Animation>();
            AnimationSources.Add(Source.UserImage, new Animation());
            AnimationSources.Add(Source.AnimationGallery, new Animation());
        }

        public static void StopCurrentThread()
        {
            if (CurrentAnimation != null && CurrentAnimation.Running)
            {
                CurrentAnimation.Running = false;
                StopCurrentImg = true;
                SendImgThread.Join();
            }
        }

        // Starts the thread if it isn't already running and some images were loaded in
        public static void Start(Source src)
        {
            Animation nextAnimation = AnimationSources[src];

            if (CurrentAnimation == null)
                CurrentAnimation = nextAnimation;

            if ((nextAnimation == CurrentAnimation && CurrentAnimation.Running)
                || !nextAnimation.HasImages()
                || !SerialManager.IsConnected)
                return;

            if (src != CurrentSource)
                StopCurrentThread();

            // Triggers IndexChanged Event so that is gets a RenderedItemBorder
            nextAnimation.CurrentImgIndex = nextAnimation.CurrentImgIndex;

            nextAnimation.Running = true;
            SendImgThread = new Thread(new ThreadStart(SendImgLoop));
            SendImgThread.Start();

            CurrentAnimation = nextAnimation;
            OnSourceChanged?.Invoke(src);
            CurrentSource = src;
        }

        // Stops the thread if it's running and joins it to the main thread
        public static void Stop(Source src)
        {
            Animation nextAnimation = AnimationSources[src];

            if (nextAnimation != CurrentAnimation)
                return;

            StopCurrentThread();
        }

        // Skips the current animation
        public static void SkipAnimation(Source src)
        {
            Animation nextAnimation = AnimationSources[src];

            if (nextAnimation != CurrentAnimation)
                return;

            CurrentAnimation.Skip();

            if (CurrentAnimation.Running)
                StopCurrentImg = true;
        }

        // Reverts to the last animation
        public static void RevertAnimation(Source src)
        {
            Animation nextAnimation = AnimationSources[src];

            if (nextAnimation != CurrentAnimation)
                return;

            CurrentAnimation.Revert();

            if (CurrentAnimation.Running)
                StopCurrentImg = true;
        }

        // Loads all the .ild files from the selected folder into the Images - list
        public static void LoadImagesFromFolder(string path)
        {
            Animation gallery = AnimationSources[Source.AnimationGallery];

            gallery.CurrentImgIndex = 0;

            ClearImages();

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            lock (gallery.Images)
            {
                FileInfo[] files = dirInfo.GetFiles("*.ild")
                    .OrderBy(f => f.Name)
                    .ToArray();

                FileInfo currentFile;
                // Reads 100 files max
                for (int i = 0; i < files.Length && i < 100; i++)
                {
                    currentFile = files[i];
                    ILDParser.LoadFromPath(currentFile.FullName, currentFile.Name, ref gallery.Images);
                }
            }

            Start(Source.AnimationGallery);
        }

        // Clears all of the loaded images
        public static void ClearImages()
        {
            Animation gallery = AnimationSources[Source.AnimationGallery];

            gallery.CurrentImgIndex = 0;
            lock (gallery.Images)
                gallery.Images.Clear();
            StopCurrentImg = true;
        }

        public static void AddImage(Source src, VectorizedImage img)
            => AnimationSources[src].Images.Add(img);

        public static Animation GetAnimation(Source src)
            => AnimationSources[src];

        static void SendImgLoop()
        {
            // This stopwatch ensures, that 24 frames are diplayed and not any more
            Stopwatch stopwatch = new Stopwatch();
            VectorizedImage currentImg;
            VectorizedFrame currentFrame;
            while (CurrentAnimation.Running)
            {
                // Setting it to false here, so not every frame after a delete call gets skipped
                StopCurrentImg = false;

                currentImg = CurrentAnimation.GetCurrentImg();
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
                while (!CurrentAnimation.HasImages()) ;
                // Moving on to the next image, or looping to the beginning
                if (!StopCurrentImg)
                    CurrentAnimation.IncrementIndex();
            }
        }
    }
}

class Animation
{
    public delegate void IndexChangedHandler(int oldVal, int newVal);
    public event IndexChangedHandler? OnImgIndexChanged;

    // List of all images which are going to be displayed one after another
    public List<VectorizedImage> Images;

    public bool Running = false;

    public int CurrentImgIndex
    {
        get => _CurrentImgIndex;
        set
        {
            OnImgIndexChanged?.Invoke(_CurrentImgIndex, value);
            _CurrentImgIndex = value;
        }
    }
    int _CurrentImgIndex;

    public Animation()
    {
        Images = new List<VectorizedImage>();
    }

    public VectorizedImage GetCurrentImg()
        => Images[CurrentImgIndex];

    public void IncrementIndex()
        => CurrentImgIndex = (CurrentImgIndex + 1) % Images.Count;

    public bool HasImages() 
        => Images.Count > 0;

    public void Skip()
    {
        if (CurrentImgIndex < Images.Count - 1)
            CurrentImgIndex++;
    }

    public void Revert()
    {
        if (Images.Count > 0 && CurrentImgIndex > 0)
            CurrentImgIndex--;
    }
}
