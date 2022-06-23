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

        int MinDistIndex = 0;
        Point[] RectCorners = new Point[4];

        public RectangleWrapper(Rectangle shape) : base(shape)
        {
            RectCorners[0] = StartPoint;
            RectCorners[1] = new Point(StartPoint.X + Shape.Width, StartPoint.Y               , true);
            RectCorners[2] = new Point(StartPoint.X + Shape.Width, StartPoint.Y + Shape.Height, true);
            RectCorners[3] = new Point(StartPoint.X              , StartPoint.Y + Shape.Height, true);
        }

        protected override (Point, Point) CalcEnds()
        {
            System.Windows.Point p = new System.Windows.Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
            return (new Point(p.X, p.Y, true), new Point(p.X, p.Y, true));
        }

        // Checks the distance to p to every corner and saves the index of the corner, which had the smallest distance
        public override double GetShortestDistance(Point p)
        {
            double dist;
            double minDist = double.MaxValue;

            for (int i = 0; i < RectCorners.Length; i++)
            {
                dist = Point.GetDistance(RectCorners[i], p);
                if (dist < minDist)
                {
                    minDist = dist;
                    MinDistIndex = i;
                }
            }

            return minDist;
        }

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            // Adds the points to the list, starting from the corner which had the smallest distance to the previous point
            Point currentPoint;
            for (int i = 0; i < RectCorners.Length + 1; i++)
            {
                currentPoint = RectCorners[(MinDistIndex + i) % RectCorners.Length];
                // The first point has to be off
                addPoint(currentPoint.X, currentPoint.Y, i == 0 ? false : true);
            }
        }
    }
}
