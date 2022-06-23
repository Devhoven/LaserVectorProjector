using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = ProjectorInterface.GalvoInterface.Point;
using WPFLine = System.Windows.Shapes.Line;

namespace LVP_Studio.Helper
{
    class LineWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new WPFLine Shape => base.Shape as WPFLine ?? null!;

        public LineWrapper(WPFLine shape) : base(shape)
        { }

        protected override (Point, Point) CalcEnds()
        {
            double xOffset = Canvas.GetLeft(Shape);
            double yOffset = Canvas.GetTop(Shape);
            return (new Point(Shape.X1 + xOffset, Shape.Y1 + yOffset, true),
                    new Point(Shape.X2 + xOffset, Shape.Y2 + yOffset, true));
        }

        // Returns the shortest distance to the given point
        // If the start point does not have the shortest distance to the point, the end point is going to be switched with the start point
        // and the line is going to be built up in "reverse"
        public override double GetShortestDistance(Point p)
        {
            double distToStart = Point.GetDistance(StartPoint, p);
            double distToEnd = Point.GetDistance(EndPoint, p);

            if (distToEnd < distToStart)
            {
                (StartPoint, EndPoint) = (EndPoint, StartPoint);
                return distToEnd;
            }

            return distToStart;
        }

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            addPoint(StartPoint.X, StartPoint.Y, false);

            addPoint(EndPoint.X, EndPoint.Y, true);
        }
    }
}
