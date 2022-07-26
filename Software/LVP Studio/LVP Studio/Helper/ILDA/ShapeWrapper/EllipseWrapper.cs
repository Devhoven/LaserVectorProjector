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

        const double MAX_ARC_LENGTH_SQUARED = 30 * 30;
        
        public EllipseWrapper(Ellipse shape) : base(shape)
        { }

        protected override (Point, Point) CalcEnds()
        {
            System.Windows.Point p = new System.Windows.Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
            return (new Point(p.X + Shape.Width, p.Y + Shape.Height / 2, true),
                    new Point(p.X + Shape.Width, p.Y + Shape.Height / 2, true));
        }

        // TODO
        public override double GetShortestDistance(Point p)
            => Point.GetDistance(StartPoint, p);

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            // moving to the top of the ellipse from where the other points are calculated
            addPoint(StartPoint.X, StartPoint.Y, false);

            addPoint(StartPoint.X, StartPoint.Y, true);

            System.Windows.Point topLeft = new System.Windows.Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));

            double r1 = Shape.Width / 2;
            double r2 = Shape.Height / 2;

            double oldX = StartPoint.X;
            double oldY = StartPoint.Y;

            // Loop until we get all the points out of the ellipse
            for (double angle = 0; angle < Math.PI * 2; angle += 0.01)
            {
                double x = r1 * Math.Cos(angle) + r1 + topLeft.X;
                double y = r2 * Math.Sin(angle) + r2 + topLeft.Y;

                if (GetDistanceSqrd(x, y, oldX, oldY) > MAX_ARC_LENGTH_SQUARED)
                {
                    addPoint(x, y, true);
                    oldX = x;
                    oldY = y;
                }
            }

            addPoint(StartPoint.X, StartPoint.Y, true);
        }

        // Calculates the squared distance between the points (x1, y1), (x2, y2)
        double GetDistanceSqrd(double x1, double y1, double x2, double y2)
            => Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
    }
}
