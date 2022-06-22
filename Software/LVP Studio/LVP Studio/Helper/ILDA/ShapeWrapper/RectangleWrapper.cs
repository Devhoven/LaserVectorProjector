using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Point = ProjectorInterface.GalvoInterface.Point;

namespace LVP_Studio.Helper
{
    class RectangleWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Rectangle Shape => base.Shape as Rectangle ?? null!;

        public RectangleWrapper(Rectangle shape) : base(shape)
        { }

        protected override (Point, Point) CalcEnds()
        {
            System.Windows.Point p = new System.Windows.Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
            return (new Point(p.X, p.Y, true), new Point(p.X, p.Y, true));
        }

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            // Moving to the top left corner
            addPoint(StartLine.X, StartLine.Y, false);

            // Top right corner
            addPoint(StartLine.X + Shape.Width, StartLine.Y, true);

            // Lower right corner
            addPoint(StartLine.X + Shape.Width, StartLine.Y + Shape.Height, true);

            // Lower left corner
            addPoint(StartLine.X, StartLine.Y + Shape.Height, true);

            // Top left corner
            addPoint(StartLine.X, StartLine.Y, true);
        }
    }
}
