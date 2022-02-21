﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectorInterface.Helper;
using ProjectorInterface.GalvoInterface;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;

namespace ProjectorInterface.GalvoInterface
{
    
    class VectorizedImage
    {
        public List<VectorizedFrame> Frames;

        public VectorizedFrame this[int i]
        { 
            get => Frames[i]; 
        }

        public ushort FrameCount => (ushort)Frames.Count;

        public VectorizedImage()
            => Frames = new List<VectorizedFrame>();

        public void AddFrame(VectorizedFrame frame)
            => Frames.Add(frame);

        public void RemoveFrame(VectorizedFrame frame)
            => Frames.Remove(frame);    
    }
}
