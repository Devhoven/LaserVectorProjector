using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static ProjectorInterface.Commands.CanvasCommand;
using static ProjectorInterface.Helper.Settings;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Line = ProjectorInterface.GalvoInterface.Line;
using WPFLine = System.Windows.Shapes.Line;

namespace ProjectorInterface.Commands
{
    static class ShapesToPoints
    {
        static List<Line> points = new List<Line>();
        static Line currentp = new Line();
        static double ellipseAccuracy = 0.001; //99.9
        static VectorizedImage tempImage = new VectorizedImage();

        public static VectorizedImage getPoints()
        {
            foreach (UIElement child in Parent.Children)
            {
                currentp.On = false;

                if (child is WPFLine line)
                {
                    // moving to (X1, Y1) with laser OFF
                    CalcCoord(true, line.X1, line.Y1);

                    // moving to (X2, Y2) with laser ON
                    CalcCoord(false, line.X2, line.Y2);
                }
                else if (child is Rectangle rectangle)
                {
                    Point p = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));

                    // moving to "upper left" corner with laser OFF
                    CalcCoord(true, p.X, p.Y);

                    // upper right corner
                    CalcCoord(true, currentp.X + rectangle.Width, currentp.Y);

                    // lower right corner
                    CalcCoord(true, currentp.X, currentp.Y + rectangle.Height);

                    // lower left corner
                    CalcCoord(true, currentp.X - rectangle.Width, currentp.Y);

                    // back to upper left
                    CalcCoord(false, currentp.X, currentp.Y - rectangle.Height);
                }
                else if (child is Ellipse ellipse)
                {
                    Point p = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));

                    // moving to the top of the ellipse from where the other points are calculated
                    CalcCoord(true, ellipse.Width + p.X, ellipse.Height / 2 + p.Y);

                    // Distance in radians between angles measured on the ellipse
                    double deltaAngle = 0.001;
                    double circumference = GetLengthOfEllipse(deltaAngle, ellipse);

                    // Length of each ellipse-arc
                    double arcLength = 150 / (2 * Math.PI);

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
                        if ((points[points.Count - 1].X == (x / CANVAS_RESOLUTION)) && (points[points.Count - 1].Y == (y / CANVAS_RESOLUTION)))
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
                    // TODO Freihandzeichnen
                }
                // Bugfix: No more connecting lines between shapes
                //CalcCoord(false, currentp.X * CANVAS_RESOLUTION, currentp.Y * CANVAS_RESOLUTION);
                //currentp.On = false;

            }

            // testpoints();

            Line[] tmp = points.ToArray();
            points.Clear();
            tempImage.AddFrame(new GalvoInterface.VectorizedFrame(tmp));

            return tempImage;
        }

        // Testing conversion to Points
        static void testPoints()
        {
            foreach (Line p in points)
            {
                Ellipse tmp = new Ellipse()
                {
                    StrokeThickness = 4,
                    Stroke = new SolidColorBrush(Colors.Blue),
                    Width = 10,
                    Height = 10
                };
                Canvas.SetLeft(tmp, p.X * CANVAS_RESOLUTION);
                Canvas.SetTop(tmp, p.Y * CANVAS_RESOLUTION);
                Parent.Children.Add(tmp);
            }
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
            currentp.X = (short)x;
            currentp.Y = (short)y;
            currentp.On = stroke;
            Line l = Line.NormalizedLine(currentp.X, currentp.Y, currentp.On, CANVAS_RESOLUTION, MAX_VOLTAGE);
            points.Add(l);

        }
    }
}
