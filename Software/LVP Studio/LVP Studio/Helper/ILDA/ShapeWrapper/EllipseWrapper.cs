using LvpStudio;
using LvpStudio.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Point = LvpStudio.GalvoInterface.Point;

namespace LVP_Studio.Helper
{
    class EllipseWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Ellipse Shape => base.Shape as Ellipse ?? null!;

        const double MAX_ARC_LENGTH_SQUARED = 20 * 20;

        Point MidPoint;
        double ShortestDistanceStartAngle = 0;
        
        public EllipseWrapper(Ellipse shape) : base(shape)
        { }

        protected override (Point, Point) CalcEnds()
        {
            System.Windows.Point p = new System.Windows.Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
            MidPoint = new Point(p.X + Shape.Width / 2, p.Y + Shape.Height / 2);
            return (new Point(p.X, p.Y), new Point(p.X, p.Y));
        }

        public override double GetShortestDistance(Point p)
        {
            Vector v = p - MidPoint;

            // Calculates the angle between the vector v and the vector (1, 0) 
            // The angle is given between 0 and PI * 2
            ShortestDistanceStartAngle = Math.Atan2(v.Y, v.X);

            StartPoint = new Point(Math.Cos(ShortestDistanceStartAngle) * Shape.Width / 2 + MidPoint.X,
                                   Math.Sin(ShortestDistanceStartAngle) * Shape.Height / 2 + MidPoint.Y);
            EndPoint = StartPoint;

            return Point.GetDistance(StartPoint, p);
        }

        // This method distributes equidistant points along the ellipse
        // It's a bit hacky, but at least no integrals have to be solved
        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            addPoint(StartPoint.X, StartPoint.Y, false);

            double r1 = Shape.Width / 2;
            double r2 = Shape.Height / 2;

            double oldX = StartPoint.X;
            double oldY = StartPoint.Y;

            // Loop until we get all the points of the ellipse
            for (double angle = 0; angle < Math.PI * 2; angle += 0.01)
            {
                double x = r1 * Math.Cos(angle + ShortestDistanceStartAngle) + MidPoint.X;
                double y = r2 * Math.Sin(angle + ShortestDistanceStartAngle) + MidPoint.Y;

                // Only add the point to the list if it is greater than the MAX_ARC_LENGTH
                // This ensures that the points are distributed equidistantly along the ellipse
                if (GetDistanceSqrd(x, y, oldX, oldY) > MAX_ARC_LENGTH_SQUARED)
                {
                    addPoint(x, y, true);
                    oldX = x;
                    oldY = y;
                }
            }

            addPoint(StartPoint.X, StartPoint.Y, true);

            // Calculates the squared distance between the points (x1, y1), (x2, y2)
            double GetDistanceSqrd(double x1, double y1, double x2, double y2)
                => Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
        }
    }
}
