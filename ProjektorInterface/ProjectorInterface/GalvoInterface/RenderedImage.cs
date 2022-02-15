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

namespace ProjectorInterface.GalvoInterface
{
    class RenderedImage : Image
    {
        VectorizedImage Image;

        public RenderedImage(VectorizedImage image, int thumbnailIndex = 0)
        {
            Image = image;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
            RenderImage(image[thumbnailIndex]);
        }

        void RenderImage(VectorizedFrame frame)
        {
            Bitmap bmp = new Bitmap(Settings.RENDERED_IMG_BMP_SIZE, Settings.RENDERED_IMG_BMP_SIZE);

            float maxVolF = Settings.MAX_VOLTAGE;

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.Clear(Color.White);
                Pen linePen = new Pen(System.Drawing.Brushes.Red, 5f);

                for (int i = 0; i < frame.Lines.Length - 1; i++)
                {
                    if (frame.Lines[i + 1].On)
                    {
                        graph.DrawLine(linePen, TransformX(i), TransformY(i), TransformX(i + 1), TransformY(i + 1));
                    }
                }

                int TransformX(int i)
                    => (int)(frame.Lines[i].X / maxVolF * bmp.Width);

                int TransformY(int i)
                    => (int)(frame.Lines[i].Y / maxVolF * bmp.Height);
            }

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bmpimage = new BitmapImage();
            bmpimage.BeginInit();
            bmpimage.StreamSource = ms;
            bmpimage.EndInit();
            Source = bmpimage;
        }
    }
}