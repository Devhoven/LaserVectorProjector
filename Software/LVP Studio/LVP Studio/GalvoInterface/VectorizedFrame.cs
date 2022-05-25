using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static ProjectorInterface.Helper.Settings;

namespace ProjectorInterface.GalvoInterface
{
    // Currently just a mockup class which is later going to contain an array of vectors which define the lines
    // It is going to be responsible for the normalization of the data and interpolation between the lines, if they are too far apart
    class VectorizedFrame
    {
        public ushort LineCount => (ushort)Lines.Length;

        // How many times this frame should be repeated
        public ushort ReplayCount = 1;

        // Array of all the finished lines
        public readonly Line[] Lines;

        public VectorizedFrame(Line[] lines)
            => Lines = lines;

        public static VectorizedFrame InterpolatedFrame(Line[] lines)
            => new VectorizedFrame(InterpolateLines(lines));

        static Line[] InterpolateLines(Line[] lines)
        {
            List<Line> interpolatedLines = new List<Line>();
            double x, y;
            short newX, newY;
            double diffX, diffY;
            double oldDiffX = 0, oldDiffY = 0;
            double oldOldDiffX = 0, oldOldDiffY = 0;
            double dist;
            double xRatio, yRatio;
            double mult;
            for (int i = 0; i < lines.Length - 1; i++)
            {
                // Retreiving the start points
                x = lines[i].X;
                y = lines[i].Y;
                // Retreiving the end points of this line segment
                newX = lines[i + 1].X;
                newY = lines[i + 1].Y;

                // Calculating the difference between the old and new coordinates
                diffX = newX - x;
                diffY = newY - y;

                // Calculating the manhattan distance
                dist = Math.Abs(diffX) + Math.Abs(diffY);

                // Ratio between the difference and the manhattan distance
                xRatio = diffX / dist;
                yRatio = diffY / dist;

                // Scaling one of the ratios up to 1, (the other one accordingly) so that the galvos move the fastest they possibly can per step
                mult = 1 / Math.Abs(Math.Abs(xRatio) > Math.Abs(yRatio) ? xRatio : yRatio);
                xRatio *= mult;
                yRatio *= mult;

                // Otherwise the first point won't be added
                if (i == 0)
                {
                    interpolatedLines.Add(new Line((short)x, (short)y, false));
                    interpolatedLines.Add(new Line((short)x, (short)y, false));
                }

                double angle = GetAngle(diffX, diffY, oldOldDiffX, oldOldDiffY);

                oldOldDiffX = oldDiffX;
                oldOldDiffY = oldDiffY;

                oldDiffX = diffX;
                oldDiffY = diffY;

                // 0.785398 = 45 degrees in radians
                if (angle > 0.785398)
                {
                    interpolatedLines.Add(new Line((short)x, (short)y, lines[i + 1].On));
                    oldOldDiffX = diffX;
                    oldOldDiffY = diffY;
                }

                // Traverses from one point to another until the distance is too short and adds the points on the way
                while (true)
                {
                    if (Math.Abs(x - newX) <= MAX_STEP_SIZE && Math.Abs(y - newY) <= MAX_STEP_SIZE)
                    {
                        interpolatedLines.Add(new Line(newX, newY, lines[i + 1].On));
                        break;
                    }
                    x += MAX_STEP_SIZE * xRatio;
                    y += MAX_STEP_SIZE * yRatio;
                    interpolatedLines.Add(new Line((short)x, (short)y, lines[i + 1].On));
                }
                
                double GetAngle(double x1, double y1, double x2, double y2)
                    => Math.Acos((x1 * x2 + y1 * y2) / (Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sqrt(x2 * x2 + y2 * y2)));
            }
            return interpolatedLines.ToArray();
        }
    }

    // This line - struct contains the x and y coordinates (normalized between 0 and MaxValue) and how many microseconds the arduino should wait 
    // It also holds, if the laser should be turned on or off
    struct Line
    {
        public short X;
        public short Y;
        public bool On;

        public Line(int x, int y, bool on)
        {
            X = (short)x;
            Y = (short)y;
            On = on;
        }

        public static Line NormalizedLine(int x, int y, bool on, int normalizeTo)
        {
            Line result;
            result.X = (short)((x + short.MaxValue) / (float)(short.MaxValue * 2) * normalizeTo);
            result.Y = (short)((y + short.MaxValue) / (float)(short.MaxValue * 2) * normalizeTo);
            result.On = on;
            return result;
        }

        public static Line NormalizedLine(int x, int y, bool on, int oldMax, int newMax)
        {
            Line result;
            result.X = (short)(x / (float)oldMax * newMax);
            result.Y = (short)(y / (float)oldMax * newMax);
            result.On = on;
            return result;
        }

        public static bool operator==(Line v1, Line v2)
            => v1.X == v2.X && v1.Y == v2.Y;

        public static bool operator !=(Line v1, Line v2)
            => !(v1 == v2);
     
        public override bool Equals(object? obj)
            => obj is Line v && v == this;

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => "(" + X + ", " + Y + ") " + On;
    }
}