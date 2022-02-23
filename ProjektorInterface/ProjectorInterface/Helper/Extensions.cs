using ProjectorInterface.GalvoInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectorInterface.Helper
{
    static class Extensions
    {
        // Extension for shapes, returns a string representation of the given shape
        public static string StrRep(this Shape shape)
        {
            if (shape is System.Windows.Shapes.Line)
                return "Line";
            if (shape is System.Windows.Shapes.Rectangle)
                return "Rectangle";
            if (shape is System.Windows.Shapes.Ellipse)
                return "Ellipse";
            if (shape is System.Windows.Shapes.Path)
                return "Path";
            if (shape is ProjectorInterface.DrawingTools.SelectionRectangle)
                return "Selection";
            return "Nothing";
        }

       public static BitmapImage GetRenderedFrame(this VectorizedFrame frame)
        {
            Bitmap bmp = new Bitmap(Settings.RENDERED_IMG_BMP_SIZE, Settings.RENDERED_IMG_BMP_SIZE);

            float maxVolF = Settings.IMG_SECTION_SIZE;

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.Clear(Color.White);
                Pen linePen = new Pen(Brushes.Red, 5f);

                for (int i = 1; i < frame.Lines.Length; i++)
                {
                    if (frame.Lines[i].On)
                        graph.DrawLine(linePen, TransformX(i - 1), TransformY(i - 1), TransformX(i), TransformY(i));
                }

                int TransformX(int i)
                {
                    int res = (int)(frame.Lines[i].X / maxVolF * bmp.Width);
                    return res;
                }

                int TransformY(int i)
                {
                    int res = (int)(frame.Lines[i].Y / maxVolF * bmp.Height);
                    return res;
                }
            }

            // Possible "Memory Leak"
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.StreamSource = ms;
            bmpImage.EndInit();
            return bmpImage;
        }
    }
}
