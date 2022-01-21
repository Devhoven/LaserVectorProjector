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
        static double ellipseAccuracy = 0.01;

        public static List<LineSegment> getPoints()
        {
            foreach (UIElement child in Parent.Children)
            {
                if (child is Line line)
                {
                    // moving to (X1, Y1) with laser OFF
                    CalcCoord(false, line.X1, line.Y1);

                    // moving to (X2, Y2) with laser ON
                    CalcCoord(true, line.X2, line.Y2);
                }
                else if (child is Rectangle rectangle)
                {
                    Point p = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));

                    // moving to "upper left" corner with laser OFF
                    CalcCoord(false, p.X, p.Y);

                    // upper right corner
                    CalcCoord(true, currentp.X + rectangle.Width, currentp.Y);

                    // lower right corner
                    CalcCoord(true, currentp.X, currentp.Y + rectangle.Height);

                    // lower left corner
                    CalcCoord(true, currentp.X - rectangle.Width, currentp.Y);

                    // back to upper left
                    CalcCoord(true, currentp.X, currentp.Y - rectangle.Height);
                }
                else if (child is Ellipse ellipse)
                {
                    Point p = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));

                    // moving to the center of the ellipse from where the other points are calculated
                    CalcCoord(false, currentp.X + ellipse.Width / 2 + p.X, currentp.Y + ellipse.Height / 2 + p.Y);

                    // Distance in radians between angles measured on the ellipse
                    double deltaAngle = 0.001;
                    double circumference = GetLengthOfEllipse(deltaAngle, ellipse);

                    // Length of each ellipse-arc
                    double arcLength = 100;

                    double angle = 0;

                    double r1 = ellipse.Width / 2;
                    double r2 = ellipse.Height / 2;

                    // Loop until we get all the points out of the ellipse
                    for (int numPoints = 0; numPoints < circumference / arcLength; numPoints++)
                    {
                        angle = GetAngleForArcLengthRecursively(0, arcLength, angle, deltaAngle, r2, r1);

                        double x = r1 * Math.Cos(angle) + r1 + p.X;
                        double y = r2 * Math.Sin(angle) + r2 + p.Y;

                        // checks if point already is inside list
                        if ((points[points.Count - 1].Point.X == (x / CANVAS_RESOLUTION)) && (points[points.Count - 1].Point.Y == (y / CANVAS_RESOLUTION)))
                        {
                            continue;
                        }
                        else
                        {
                            CalcCoord(true, x, y);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            // TESTING CONVERSION TO POINTS
            /*
            foreach (LineSegment ls in points)
            {
                Ellipse tmp = new Ellipse()
                {
                    StrokeThickness = 4,
                    Stroke = new SolidColorBrush(Colors.Blue),
                    Width = 10,
                    Height = 10
                };
                Canvas.SetLeft(tmp, ls.Point.X * CANVAS_RESOLUTION);
                Canvas.SetTop(tmp, ls.Point.Y * CANVAS_RESOLUTION);
                Parent.Children.Add(tmp);
            }*/

            return points;
        }

        // Calculates the curcumference of a given Ellipse 
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

        // Calculates the angle for the next arc piece
        private static double GetAngleForArcLengthRecursively(double currentArcPos, double goalArcPos, double angle, double angleSeg, double maj, double min)
        {
            // Accuracy for when we are overshooting/undershooting
            double ARC_ACCURACY = ellipseAccuracy;
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

        // Calculates arc lengh of an Ellipse with a given angle as a line
        private static double ComputeArcOverAngle(double r1, double r2, double angle, double angleSeg)
        {
            double dpt_sin = Math.Pow(r1 * Math.Sin(angle), 2.0);
            double dpt_cos = Math.Pow(r2 * Math.Cos(angle), 2.0);
            double distance = Math.Sqrt(dpt_sin + dpt_cos);

            // Scale the value of distance
            return distance * angleSeg;
        }

        // Adds a new LineSegment towards the normalized point (x,y) with the laserstatus stroke
        static void CalcCoord(bool stroke, double x, double y)
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
