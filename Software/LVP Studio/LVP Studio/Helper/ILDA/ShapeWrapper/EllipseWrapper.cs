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
    class EllipseWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Ellipse Shape => base.Shape as Ellipse ?? null!;

        const double DELTA_ANGLE = 0.0001;
        const double EllipseAccuracy = 0.001; //99.9%

        public EllipseWrapper(Ellipse shape) : base(shape)
        { }

        protected override (Line, Line) CalcEnds()
        {
            Point p = new Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
            return (new Line(p.X + Shape.Width, p.Y + Shape.Height / 2, true),
                    new Line(p.X + Shape.Width, p.Y + Shape.Height / 2, true));
        }

        // The circumference
        protected override double CalcLength()
            => GetLengthOfEllipse(DELTA_ANGLE, Shape);

        public override void AddPoints(Action<double, double, bool> addLine)
        {
            // moving to the top of the ellipse from where the other points are calculated
            addLine(StartLine.X, StartLine.Y, false);

            addLine(StartLine.X, StartLine.Y, true);

            Point topLeft = new Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));

            // Length of each ellipse-arc
            double arcLength = 150 / (2 * Math.PI);

            double angle = 0;

            double r1 = Shape.Width / 2;
            double r2 = Shape.Height / 2;

            int end = (int)Math.Ceiling(Length / arcLength);

            // Loop until we get all the points out of the ellipse
            for (int numPoints = 0; numPoints < end; numPoints++)
            {
                angle = GetAngleForArcLengthRecursively(0, arcLength, angle, DELTA_ANGLE, r2, r1);

                double x = r1 * Math.Cos(angle) + r1 + topLeft.X;
                double y = r2 * Math.Sin(angle) + r2 + topLeft.Y;

                addLine(x, y, true);
            }

            addLine(StartLine.X, StartLine.Y, true);
            addLine(StartLine.X, StartLine.Y, false);
            addLine(StartLine.X, StartLine.Y, false);
            addLine(StartLine.X, StartLine.Y, false);
        }

        // Calculates the angle for the next arc piece
        private static double GetAngleForArcLengthRecursively(double currentArcPos, double goalArcPos, double angle, double angleSeg, double maj, double min)
        {
            // Accuracy for when we are overshooting/undershooting
            double ARC_ACCURACY = EllipseAccuracy;
            // Calculate arc length at new angle
            double nextSegLength = ComputeArcOverAngle(maj, min, angle + angleSeg, angleSeg);

            if (currentArcPos + nextSegLength > goalArcPos)
            {
                // If we've overshot, reduce the delta angle and try again
                return GetAngleForArcLengthRecursively(currentArcPos, goalArcPos, angle, angleSeg / 2, maj, min);
            }
            else if (currentArcPos + nextSegLength < goalArcPos - ((goalArcPos - currentArcPos) * ARC_ACCURACY))
            {
                // We're below our goal value but not in range
                return GetAngleForArcLengthRecursively(currentArcPos + nextSegLength, goalArcPos, angle + angleSeg, angleSeg, maj, min);
            }
            else
            {
                // Current arc length is in range (within our Accuracy), so return the angle
                return angle;
            }
        }

        // Calculates the circumference of a given Ellipse 
        private static double GetLengthOfEllipse(double deltaAngle, Ellipse child)
        {
            // Distance in radians between angles
            double numIntegrals = Math.Round(Math.PI * 2.0 / deltaAngle);

            double radiusX = child.Width / 2;
            double radiusY = child.Height / 2;
            double length = 0;

            // integrate over the elipse to get the circumference
            for (int i = 0; i < numIntegrals; i++)
            {
                length += ComputeArcOverAngle(radiusX, radiusY, i * deltaAngle, deltaAngle);
            }

            return length;
        }

        // Calculates arc lengh of an Ellipse with a given angle as a line
        private static double ComputeArcOverAngle(double r1, double r2, double angle, double angleSeg)
        {
            double dpt_sin = Math.Pow(r1 * Math.Sin(angle), 2.0);
            double dpt_cos = Math.Pow(r2 * Math.Cos(angle), 2.0);
            double distance = Math.Sqrt(dpt_sin + dpt_cos);

            // Scale the value of distance
            return distance * angleSeg;
        }
    }
}
