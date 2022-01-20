using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectorInterface.DrawingTools
{
    // Allows the user to draw a line
    class LineTool : DrawingTool
    {
        Line LineObj => (Line)Current;

        public LineTool() : base(new Line())
        {
            LineObj.X1 = 0;
            LineObj.Y1 = 0;
            LineObj.X2 = 0;
            LineObj.Y2 = 0;

            // Otherwise the line can't be seen, since the width and height of the line object are never altered
            Current.Width = double.MaxValue;
            Current.Height = double.MaxValue;
        }

        public override Shape CopyShape()
        {
            return new Line()
            {
                StrokeThickness = Settings.SHAPE_THICKNESS,
                Stroke = Settings.SHAPE_COLOR,
                X1 = LineObj.X1,
                Y1 = LineObj.Y1,
                X2 = LineObj.X2,
                Y2 = LineObj.Y2
            };
        }

        public override void Render(Point start, Point end)
        {
            LineObj.X1 = start.X;
            LineObj.Y1 = start.Y;
            LineObj.X2 = end.X;
            LineObj.Y2 = end.Y;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (Math.Abs(end.X - start.X) < Math.Abs(end.Y - start.Y))
                    LineObj.X2 = LineObj.X1;
                else
                    LineObj.Y2 = LineObj.Y1;
            }
        }
    }
}
