using System;
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

            // Searches for the image with the most lines and displays it 
            // The thumbnail is sometimes empty, so with this method it is guaranteed to show an image
            int maxPoints = 0;
            int maxPointsIndex = 0;
            for (int i = 0; i < image.FrameCount; i++)
            {
                if (maxPoints < image[i].PointCount)
                {
                    maxPoints = image[i].PointCount;
                    maxPointsIndex = i;
                }
            }
            Source = image[maxPointsIndex].GetRenderedFrame();
            //using (var fileStream = new FileStream("C:/Users/Vincent/Pictures/ILDATest.png", FileMode.Create))
            //{
            //    BitmapEncoder encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)Source));
            //    encoder.Save(fileStream);
            //}
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
        }
    }
}