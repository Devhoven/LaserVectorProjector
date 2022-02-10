using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using ProjectorInterface.Helper;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System;

namespace ProjectorInterface.GalvoInterface
{
    class RenderedImage : Image
    {
        VectorizedImage frame;
        public RenderedImage(VectorizedAnimation vecAn)
        {
            frame = vecAn.getImageAt(39);

            CreateImage();
        }

        public RenderedImage(LineSegment[] l)
        {
            ContextMenu = new ContextMenu();
            MenuItem DeleteImageItem = new MenuItem()
            {
                Header = "Delete Image"
            };

            ContextMenu.Items.Add(DeleteImageItem);

            DeleteImageItem.Click += DeleteImageClick;
            MouseLeftButtonDown += OnImageClick;
            
            CreateImage(l);
        }


        private void DeleteImageClick(object sender, RoutedEventArgs e)
        {
            // TODO Child entfernen
            
        }

        //Clicking on Images to load them into the Canvas
        private void OnImageClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                
            }
        }

        void CreateImage(LineSegment[] lines)
        {
            Bitmap bmp = new Bitmap(200, 200);

            float max_vol = Settings.MAX_VOLTAGE;
            float res = Settings.CANVAS_RESOLUTION;

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.Clear(Color.White);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines.ElementAt(i - 1).IsStroked)
                    {
                        graph.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Red, 1.0f),
                            (int)(lines[i-1].Point.X * bmp.Width), (int)(lines[i - 1].Point.Y * bmp.Height),
                            (int)(lines[i].Point.X * bmp.Width), (int)(lines[i].Point.Y * bmp.Height));
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage bmpimage = new BitmapImage();
            bmpimage.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            bmpimage.StreamSource = ms;
            bmpimage.EndInit();
            
            Source = bmpimage;
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
                    if (frame.Lines[i - 1].On)
                    {
                        graph.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Red, 1.0f),
                            (int)(frame.Lines[i - 1].X / max_vol * bmp.Width), (int)(frame.Lines[i - 1].Y / max_vol * bmp.Height),
                            (int)(frame.Lines[i].X / max_vol * bmp.Width), (int)(frame.Lines[i].Y / max_vol * bmp.Height));
                    }
                }
            }
            


            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage bmpimage = new BitmapImage();
            bmpimage.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            bmpimage.StreamSource = ms;
            bmpimage.EndInit();

            Source = bmpimage;
        }
    }
}