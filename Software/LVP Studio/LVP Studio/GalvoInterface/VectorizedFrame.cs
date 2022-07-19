using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static ProjectorInterface.Helper.Settings;

namespace ProjectorInterface.GalvoInterface
{
    // Currently just a mockup class which is later going to contain an array of vectors which define the lines
    // It is going to be responsible for the normalization of the data and interpolation between the points, if they are too far apart
    class VectorizedFrame
    {
        public ushort PointCount => (ushort)Points.Length;

        // How many times this frame should be repeated
        public ushort ReplayCount = 1;

        // Array of all the finished points
        public readonly Point[] Points;

        public VectorizedFrame(Point[] points)
            => Points = points;

        public static VectorizedFrame InterpolatedFrame(Point[] points)
            => new VectorizedFrame(InterpolatePoints(points));

        static Point[] InterpolatePoints(Point[] points)
        {
            if (points.Length == 0)
                return points;

            CalcDistances(points, out double travelDist, out double travelOffDist);

            // Amount of points, which are available for each frame
            int pointsPerFrame = SCAN_SPEED / SCAN_FPS;
            
            double totalOffPoints = travelOffDist / OFF_LINE_MAX_STEP_SIZE;

            // (pointsPerFrame - totalOffPoints) is the amount of points, which are still available, per frame
            // (travelDist - travelOffDist) is the total distance the laser travels, while turned on
            double onPointsPerDist = (pointsPerFrame - totalOffPoints) / (travelDist - travelOffDist);

            List<Point> interpolatedPoints = new List<Point>();
            double x, y;
            short nextX, nextY;
            double diffX, diffY;
            double prevDiffX = 0, prevDiffY = 0;
            double dist;
            double xRatio, yRatio;
            double angle;
            bool lineOn;

            // Adding the first point
            // The length of "points" is always > 0
            interpolatedPoints.Add(points[0]);

            for (int i = 0; i < points.Length - 1; i++)
            {
                // Retreiving the start coords
                x = points[i].X;
                y = points[i].Y;
                // Retreiving the end coords of this line segment
                nextX = points[i + 1].X;
                nextY = points[i + 1].Y;
                lineOn = points[i + 1].On;

                // Calculating the difference between the old and new coordinates
                diffX = nextX - x;
                diffY = nextY - y;

                // Calculating the distance 
                dist = Math.Sqrt(diffX * diffX + diffY * diffY);

                // Ratio between the difference and the distance
                xRatio = diffX / dist;
                yRatio = diffY / dist;

                angle = GetAngle(diffX, diffY, prevDiffX, prevDiffY);

                prevDiffX = diffX;
                prevDiffY = diffY;

                // If the angle between two lines is too great, two points are added, so that the galvos and the laser have enough time to adjust 
                if (angle > ADJUST_ANGLE)
                {
                    interpolatedPoints.Add(new Point((short)x, (short)y, false));
                    interpolatedPoints.Add(new Point((short)x, (short)y, false));
                }

                if (lineOn)
                    InterpolatePoints(CalcLin, (int)(onPointsPerDist * dist));
                else
                    InterpolatePoints(CalcParab, (int)(dist / OFF_LINE_MAX_STEP_SIZE));

                // The function for a equidistant distribution of points
                double CalcLin(double x)
                    => x;

                // This function is the integral of -2|x*2 - 1| + 2
                // This means, that the points have a higher density at the end and start point than in the middle
                double CalcParab(double x)
                    => 2 * ((x * x - x + 0.25) * Math.Sign(0.5 - x) + x - 0.25);

                // Returns the angle between two vectors
                double GetAngle(double x1, double y1, double x2, double y2)
                    => Math.Acos((x1 * x2 + y1 * y2) / (Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sqrt(x2 * x2 + y2 * y2)));

                // Interpolates between two points, with the help of the f - function (either a equidistant distribution or whatever you need) 
                // f - function requirements: f(0) = 0; f(1) = 1; f(x) <= 1; f(x) >= 0
                // availablePoints determines how many points are available for the line between the two points
                void InterpolatePoints(Func<double, double> f, int availablePoints)
                {
                    // If there are less or equal than 1 points available for this line, nothing is going to be interpolated
                    if (availablePoints <= 1)
                    {
                        interpolatedPoints.Add(points[i]);
                        interpolatedPoints.Add(points[i + 1]);
                        return;
                    }

                    double stepSize = 1.0 / (availablePoints - 1);

                    double stepSizeFunc;

                    for (int j = 1; j < availablePoints; j++)
                    {
                        stepSizeFunc = f(j * stepSize) * dist;
                        interpolatedPoints.Add(
                            new Point((short)(x + stepSizeFunc * xRatio), 
                                      (short)(y + stepSizeFunc * yRatio), 
                                      lineOn));
                    }
                }
            }

            return AddStatusOffset(interpolatedPoints.ToArray());
        }

        // It calculates the distance the laser has to travel to draw the whole picture, as well as the distance where the laser is turned off
        static void CalcDistances(Point[] points, out double travelDist, out double travelOffDist)
        {
            // The total distance traveled
            travelDist = 0;
            // The distance traveled, where the laser is turned off
            travelOffDist = 0;

            if (points.Length == 0)
                return;

            Point prevPoint;
            Point currentPoint;

            double pointDist;

            for (int i = 1; i < points.Length; i++)
            {
                prevPoint = points[i - 1];
                currentPoint = points[i];

                pointDist = Point.GetDistance(prevPoint, currentPoint);

                if (currentPoint.On)
                    travelDist += pointDist;
                else
                    travelOffDist += pointDist;
            }

            // Adding the distance where the laser is off to the total
            travelDist += travelOffDist;
        }
    
        // If the laser status changes, a few prevous points are going to be changed to the new status 
        // This removes the laser "smear"
        static Point[] AddStatusOffset(Point[] points)
        {
            bool prevStatus;
            for (int i = 1; i < points.Length; i++)
            {
                prevStatus = points[i - 1].On;

                if (points[i].On != prevStatus)
                {
                    for (int j = i; j < i + 3 && j < points.Length; j++)
                        points[j].On = prevStatus;

                    i += 3;
                }
            }
            return points;
        }
    }

    // This point - struct contains the x and y coordinates (normalized between 0 and MaxValue) and if the laser should be turned on or off
    struct Point
    {
        public short X;
        public short Y;
        public bool On;

        public Point(int x, int y, bool on)
        {
            X = (short)x;
            Y = (short)y;
            On = on;
        }

        public Point(double x, double y, bool on)
        {
            X = (short)x;
            Y = (short)y;
            On = on;
        }

        public static Point NormalizedPoint(int x, int y, bool on, int normalizeTo)
        {
            Point result;
            result.X = (short)((x + short.MaxValue) / (float)(short.MaxValue * 2) * normalizeTo);
            result.Y = (short)((y + short.MaxValue) / (float)(short.MaxValue * 2) * normalizeTo);
            result.On = on;
            return result;
        }

        public static Point NormalizedPoint(int x, int y, bool on, int oldMax, int newMax)
        {
            Point result;
            result.X = (short)((float)x / oldMax * newMax);
            result.Y = (short)((float)y / oldMax * newMax);
            result.On = on;
            return result;
        }

        public static bool operator==(Point v1, Point v2)
            => v1.X == v2.X && v1.Y == v2.Y;

        public static bool operator !=(Point v1, Point v2)
            => !(v1 == v2);

        public static double GetDistance(Point v1, Point v2)
            => Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
     
        public override bool Equals(object? obj)
            => obj is Point v && v == this;

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => "(" + X + ", " + Y + ") " + On;
    }
}