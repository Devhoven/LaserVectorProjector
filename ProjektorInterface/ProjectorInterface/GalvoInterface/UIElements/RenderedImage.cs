﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Pen = System.Drawing.Pen;
using ProjectorInterface.Helper;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace ProjectorInterface.GalvoInterface.UiElements
{
    class RenderedImage : Image
    {
        public RenderedImage(VectorizedImage image)
        {
            ToolTip = new ToolTip()
            {
                Content = image.FileName
            };
            Source = image[0].GetRenderedFrame();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
        }
    }
}