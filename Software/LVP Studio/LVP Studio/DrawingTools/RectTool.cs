using LvpStudio.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LvpStudio.DrawingTools
{
    // Allows the user to draw an axis-aligned rectangle
    class RectTool : DrawingTool
    {
        Rectangle RectObj => (Rectangle)Current;

        Point RectPos;

        public RectTool() : base(new Rectangle())
        { }

        public override Shape CopyShape()
        {
            Rectangle tmp = new Rectangle()
            {
                StrokeThickness = Settings.SHAPE_THICKNESS,
                Stroke = Settings.SHAPE_COLOR,
                Width = RectObj.Width,
                Height = RectObj.Height
            };
            Canvas.SetLeft(tmp, RectPos.X);
            Canvas.SetTop(tmp, RectPos.Y);
            return tmp;
        }

        protected override void _Render(Point start, Point end)
        {
            Point topLeft = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point bottomRight = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double size = Math.Max(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                RectObj.Width = size;
                RectObj.Height = size;
            }
            else
            {
                RectObj.Width = bottomRight.X - topLeft.X;
                RectObj.Height = bottomRight.Y - topLeft.Y;
            }

            RectPos.X = topLeft.X;
            RectPos.Y = topLeft.Y;
            Canvas.SetLeft(this, RectPos.X);
            Canvas.SetTop(this, RectPos.Y);
        }
    }
}
