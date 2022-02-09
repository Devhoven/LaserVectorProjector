using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using ProjectorInterface.Helper;

namespace ProjectorInterface.GalvoInterface
{
    class RenderedImage : Image
    {
        VectorizedImage frame;
        public RenderedImage(VectorizedAnimation vecAn)
        {
            frame = vecAn.getImageAt(220);
            CreateImage();
        }

        void CreateImage()
        {
            Bitmap bmp = new Bitmap(200, 200);

            float max_vol = Settings.MAX_VOLTAGE;

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.Clear(Color.White);
                for (int i = 1; i < frame.Lines.Length; i++)
                {
                    if (frame.Lines[i -1].On)
                    {
                        graph.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Red, 1.0f),
                            (int)(frame.Lines[i - 1].X / max_vol * bmp.Width), (int)(frame.Lines[i - 1].Y / max_vol * bmp.Height),
                            (int)(frame.Lines[i].X / max_vol * bmp.Width), (int)(frame.Lines[i].Y / max_vol * bmp.Height));
                    }
                }
            }
            

            bmp.Save("C:/Users/Lukas/Pictures/Test.png");

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage bmpimage = new BitmapImage();
            bmpimage.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            bmpimage.StreamSource = ms;
            bmpimage.EndInit();

            // TODO Methodenaufruf zum zeichnen auf der Bitmap
            //var target = new RenderTargetBitmap(bmpimage.PixelWidth, bmpimage.PixelHeight, bmpimage.DpiX, bmpimage.DpiY, PixelFormats.Pbgra32);
            //var visual = new DrawingVisual();

            //target.Render(visual);

            Source = bmpimage;

            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(target));

            //FileStream fs = File.Open("C:/Users/Lukas/Downloads/test.png", FileMode.Create);
            //encoder.Save(fs);
            //fs.Close();
        }
    }
}