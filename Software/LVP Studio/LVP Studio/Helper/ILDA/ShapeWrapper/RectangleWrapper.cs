using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Line = ProjectorInterface.GalvoInterface.Line;

namespace LVP_Studio.Helper
{
    class RectangleWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Rectangle Shape => base.Shape as Rectangle ?? null!;

        public RectangleWrapper(Rectangle shape) : base(shape)
        { }

        protected override (Line, Line) CalcEnds()
        {
            Point p = new Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
            return (new Line(p.X, p.Y, true), new Line(p.X, p.Y, true));
        }

        protected override double CalcLength()
            => Shape.Width * 2 + Shape.Height * 2;

        public override void AddPoints(Action<double, double, bool> addLine)
        {
            // Moving to the top left corner
            addLine(StartLine.X, StartLine.Y, false);

            // Top right corner
            addLine(StartLine.X + Shape.Width, StartLine.Y, true);

            // Lower right corner
            addLine(StartLine.X + Shape.Width, StartLine.Y + Shape.Height, true);

            // Lower left corner
            addLine(StartLine.X, StartLine.Y + Shape.Height, true);

            // Top left corner
            addLine(StartLine.X, StartLine.Y, true);
        }
    }
}
