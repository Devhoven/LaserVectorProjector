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

            double maxVolF = Settings.IMG_SECTION_SIZE;

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.Clear(Color.White);
                Pen linePen = new Pen(Brushes.Red, 5f);

                int count = 0;

                int pointSize = 8;

                //graph.FillEllipse(Brushes.Red, TransformX(0) - pointSize, TransformY(0) - pointSize, pointSize * 2, pointSize * 2);
                for (int i = 1; i < frame.Lines.Length; i++)
                {
                    if (frame.Lines[i].On)
                    {
                        graph.DrawLine(linePen, TransformX(i - 1), TransformY(i - 1), TransformX(i), TransformY(i));
                        //graph.FillEllipse(Brushes.Green, TransformX(i) - pointSize / 2, TransformY(i) - pointSize / 2, pointSize, pointSize);
                    }
                    //else
                    //    graph.FillEllipse(Brushes.Blue, TransformX(i) - pointSize / 2, TransformY(i) - pointSize / 2, pointSize, pointSize);

                    //if (frame.Lines[i] == frame.Lines[i - 1])
                    //    count += 3;
                    //else
                    //{
                    //    if (count > 0)
                    //    {
                    //        graph.FillEllipse(Brushes.Yellow, TransformX(i - 1) - pointSize / 2, TransformY(i - 1) - pointSize / 2, pointSize, pointSize);
                    //        count = 0;
                    //    }
                    //}
                }

                int TransformX(int i)
                    => (int)(frame.Lines[i].X / maxVolF * bmp.Width);

                int TransformY(int i)
                    => (int)(frame.Lines[i].Y / maxVolF * bmp.Height);
            }

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
