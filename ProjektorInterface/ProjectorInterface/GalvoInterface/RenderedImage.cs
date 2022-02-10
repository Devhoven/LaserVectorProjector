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

        public RenderedImage(VectorizedImage image)
        {
            Image = image;
            Margin = new Thickness(20, 10, 20, 10);
            RenderImage(image[0]);
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

        void RenderImage(VectorizedFrame frame)
        {
            Bitmap bmp = new Bitmap(500, 500);

            float maxVolF = Settings.MAX_VOLTAGE;

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.Clear(Color.White);
                Pen linePen = new Pen(System.Drawing.Brushes.Red, 5f);

                for (int i = 0; i < frame.Lines.Length - 1; i++)
                {
                    if (frame.Lines[i].On)
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