using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static ProjectorInterface.DrawingCommands.CanvasCommand;
using static ProjectorInterface.Helper.Settings;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using WPFLine = System.Windows.Shapes.Line;

namespace ProjectorInterface.GalvoInterface
{
    static class ShapesToPoints
    {
        public static VectorizedImage DrawnImage = new VectorizedImage();
        static readonly int CanvasResolution;

        static List<Line> Lines = new List<Line>();
        static Line CurrentLine = new Line();

        static double EllipseAccuracy = 0.001; //99.9%

        static ShapesToPoints()
            => CanvasResolution = (int)MainWindow.Instance.DrawCon.ActualWidth;

        public static void CalcFrameFromCanvas()
        {
            foreach (UIElement child in Parent.Children)
            {
                CurrentLine.On = false;

                if (child is WPFLine line)
                {
                    if (Math.Abs(CurrentLine.X - line.X1) + Math.Abs(CurrentLine.Y - line.Y1) < 10)
                        CalcCoord(true, line.X1, line.Y1);
                    else
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
                    CalcCoord(true, CurrentLine.X + rectangle.Width, CurrentLine.Y);

                    // lower right corner
                    CalcCoord(true, CurrentLine.X, CurrentLine.Y + rectangle.Height);

                    // lower left corner
                    CalcCoord(true, CurrentLine.X - rectangle.Width, CurrentLine.Y);

                    // back to upper left
                    CalcCoord(true, CurrentLine.X, CurrentLine.Y - rectangle.Height);
                }
                else if (child is Ellipse ellipse)
                {
                    Point p = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));

                    // moving to the top of the ellipse from where the other points are calculated
                    CalcCoord(false, ellipse.Width + p.X, ellipse.Height / 2 + p.Y);

                    // Distance in radians between angles measured on the ellipse
                    double deltaAngle = 0.001;
                    double circumference = GetLengthOfEllipse(deltaAngle, ellipse);

                    // Length of each ellipse-arc
                    double arcLength = 150 / (2 * Math.PI);

                    double angle = 0;

                    double r1 = ellipse.Width / 2;
                    double r2 = ellipse.Height / 2;

                    int end = (int)Math.Ceiling(circumference / arcLength);

                    // Loop until we get all the points out of the ellipse
                    for (int numPoints = 0; numPoints < end; numPoints++)
                    {
                        angle = GetAngleForArcLengthRecursively(0, arcLength, angle, deltaAngle, r2, r1);

                        double x = r1 * Math.Cos(angle) + r1 + p.X;
                        double y = r2 * Math.Sin(angle) + r2 + p.Y;

                        // checks if point already is inside list
                        if ((Lines[Lines.Count - 1].X == (x / CanvasResolution)) && (Lines[Lines.Count - 1].Y == (y / CanvasResolution)))
                        {
                            continue;
                        }
                        else
                        {
                            CalcCoord(true, x, y);
                        }
                    }
                }
                else if (child is Path path)
                {
                    GeometryCollection lineSegments = ((GeometryGroup)path.Data).Children;
                    LineGeometry currentLine = (LineGeometry)lineSegments[0];
                    CalcCoord(false, currentLine.StartPoint.X, currentLine.StartPoint.Y);
                    for (int i = 1; i < lineSegments.Count; i++)
                    {
                        currentLine = (LineGeometry)lineSegments[i];

                        // moving to (X2, Y2) with laser ON
                        CalcCoord(true, currentLine.EndPoint.X, currentLine.EndPoint.Y);
                    }
                }
            }

            if (Lines.Count > 0)
            {
                Lines.Add(new Line(Lines[Lines.Count - 1].X, Lines[Lines.Count - 1].Y, false));
                Lines.Add(new Line(Lines[0].X, Lines[0].Y, false));
            }
            
            DrawnImage.AddFrame(VectorizedFrame.InterpolatedFrame(Lines.ToArray()));
            Lines.Clear();
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
            CurrentLine.X = (short)x;
            CurrentLine.Y = (short)y;
            CurrentLine.On = stroke;
            Lines.Add(Line.NormalizedLine(CurrentLine.X, CurrentLine.Y, CurrentLine.On, CanvasResolution, IMG_SECTION_SIZE));
        }
    }
}
