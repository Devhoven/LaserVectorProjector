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
            int maxLines = 0;
            int maxLinesIndex = 0;
            for (int i = 0; i < image.FrameCount; i++)
            {
                if (maxLines < image[i].LineCount)
                {
                    maxLines = image[i].LineCount;
                    maxLinesIndex = i;
                }
            }
            Source = image[maxLinesIndex].GetRenderedFrame();
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