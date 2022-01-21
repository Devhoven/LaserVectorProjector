using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static ProjectorInterface.Commands.CanvasCommand;
using static ProjectorInterface.Helper.Settings;

namespace ProjectorInterface.Commands
{
    static class ShapesToPoints
    {
        public static List<LineSegment> points = new List<LineSegment>();
        static LineSegment seg = new LineSegment();
        static Point currentp = new Point();
        static double precision = 4;

        public static List<LineSegment> getPoints()
        {
            foreach (UIElement child in Parent.Children)
            {
                if (child is Line)
                {
                    // moving to (X1, Y1) with laser OFF
                    seg.IsStroked = false;
                    currentp.X = ((Line)child).X1;
                    currentp.Y = ((Line)child).Y1;
                    seg.Point = currentp;
                    points.Add(seg.Clone());

                    // moving to (X2, Y2) with laser ON
                    seg.IsStroked = true;
                    currentp.X = ((Line)child).X2;
                    currentp.Y = ((Line)child).Y2;
                    seg.Point = currentp;
                    points.Add(seg.Clone());
                }
                else if (child is Rectangle)
                {
                    Point p = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));//Parent.TranslatePoint(new Point(0, 0), ((Rectangle)child));
                    // moving to "upper left" corner with laser OFF
                    calcCoord(false, p.X, p.Y);

                    // "upper right" corner
                    calcCoord(true, currentp.X + ((Rectangle)child).Width, currentp.Y);

                    // "lower right" corner
                    calcCoord(true, currentp.X, currentp.Y + ((Rectangle)child).Height);

                    // "lower left" corner
                    calcCoord(true, currentp.X - ((Rectangle)child).Width, currentp.Y);

                    // back to "upper left"
                    calcCoord(true, currentp.X, currentp.Y - ((Rectangle)child).Height);
                }
                else if (child is Ellipse)
                {
                    Point p = Parent.TranslatePoint(new Point(0, 0), ((Ellipse)child));

                    // moving to center with laser OFF
                    calcCoord(false, p.X, p.Y);
                    // moving to the north ("12 o' clock")
                    calcCoord(false, currentp.X, currentp.Y - ((Ellipse)child).Height / 2);


                    //calcCoord(true, currentp.X + Math.Sin(precision / (2*Math.PI)), currentp.Y * Math.Cos(precision/ (Math.PI)));



                    List<Point> pointsInEllipse = new List<Point>();

                    // Distance in radians between angles measured on the ellipse
                    double deltaAngle = 0.001;
                    double circumference = GetLengthOfEllipse(deltaAngle, (Ellipse)child);

                    double arcLength = 0.1;

                    double angle = 0;

                    double r1 = ((Ellipse)child).Width;
                    double r2 = ((Ellipse)child).Height;

                    if (r1 > r2)
                    {
                        double temp = r1;
                        r1 = r2;
                        r2 = temp;
                    }

                    // Loop until we get all the points out of the ellipse
                    for (int numPoints = 0; numPoints < circumference / arcLength; numPoints++)
                    {
                        angle = GetAngleForArcLengthRecursively(0, arcLength, angle, deltaAngle, r2, r1);

                        double x = r1 * Math.Cos(angle);
                        double y = r2 * Math.Sin(angle);
                        calcCoord(true, x, y);
                    }

                }
                else
                {
                    continue;
                }
            }
            return points;
        }


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

        private static double GetAngleForArcLengthRecursively(double currentArcPos, double goalArcPos, double angle, double angleSeg, double maj, double min)
        {
            double ARC_ACCURACY = 1;
            // Calculate arc length at new angle
            double nextSegLength = ComputeArcOverAngle(maj, min, angle + angleSeg, angleSeg);

            // If we've overshot, reduce the delta angle and try again
            if (currentArcPos + nextSegLength > goalArcPos)
            {
                return GetAngleForArcLengthRecursively(currentArcPos, goalArcPos, angle, angleSeg / 2, maj, min);

                // We're below the our goal value but not in range (
            }
            else if (currentArcPos + nextSegLength < goalArcPos - ((goalArcPos - currentArcPos) * ARC_ACCURACY))
            {
                return GetAngleForArcLengthRecursively(currentArcPos + nextSegLength, goalArcPos, angle + angleSeg, angleSeg, maj, min);

                // current arc length is in range (within error), so return the angle
            }
            else
                return angle;
        }

        private static double ComputeArcOverAngle(double r1, double r2, double angle, double angleSeg)
        {
            double distance = 0.0;

            double dpt_sin = Math.Pow(r1 * Math.Sin(angle), 2.0);
            double dpt_cos = Math.Pow(r2 * Math.Cos(angle), 2.0);
            distance = Math.Sqrt(dpt_sin + dpt_cos);

            // Scale the value of distance
            return distance * angleSeg;
        }




        // Adds a new LineSegment towards the point (x,y) with the laserstatus stroke to the list
        static void calcCoord(bool stroke, double x, double y)
        {
            seg.IsStroked = stroke;
            currentp.X = x;
            currentp.Y = y;
            Point normalized = new Point(currentp.X / CANVAS_RESOLUTION, currentp.Y / CANVAS_RESOLUTION);
            seg.Point = normalized;
            points.Add(seg.Clone());
        }

    }
}
