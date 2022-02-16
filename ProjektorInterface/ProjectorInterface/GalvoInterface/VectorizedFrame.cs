using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static ProjectorInterface.Helper.Settings;

namespace ProjectorInterface.GalvoInterface
{
    // Currently just a mockup class which is later going to contain an array of vectors which define the lines
    // It is going to be responsible for the normalization of the data and interpolation between the lines, if they are too far apart
    class VectorizedFrame
    {
        // Array of all the finished lines
        public readonly Line[] Lines;

        public VectorizedFrame(params Line[] lines)
        {
            List<Line> interpolatedLines = new List<Line>();
            short x, y;
            short newX, newY;
            short diffX, diffY;
            float dist;
            float xRatio, yRatio;
            float mult;
            for (int i = 0; i < lines.Length - 1; i++)
            {
                // Retreiving the start points
                x = lines[i].X;
                y = lines[i].Y;
                // Retreiving the end points of this line segment
                // Loops around, so that the last points are connected to the first
                newX = lines[i + 1].X;
                newY = lines[i + 1].Y;

                // Calculating the difference between the old and new coordinates
                diffX = (short)(newX - x);
                diffY = (short)(newY - y);

                // Calculating the manhattan distance
                dist = MathF.Abs(diffX) + Math.Abs(diffY);

                // Ratio between the difference and the manhattan distance
                xRatio = diffX / dist;
                yRatio = diffY / dist;

                // Scaling one of the ratios up to 1, (the other one accordingly) so that the galvos move the fastest they possibly can per step
                mult = 1 / MathF.Abs(MathF.Abs(xRatio) > MathF.Abs(yRatio) ? xRatio : yRatio);
                xRatio *= mult;
                yRatio *= mult;

                // Otherwise the first point won't be added
                if (i == 0)
                    interpolatedLines.Add(new Line(x, y, false));

                // Traverses from one point to another until the distance is too short and adds the points on the way
                while (true)
                {
                    x += (short)(MAX_STEP_SIZE * xRatio);
                    y += (short)(MAX_STEP_SIZE * yRatio);
                    if (Math.Abs(x - newX) <= MAX_STEP_SIZE && Math.Abs(y - newY) <= MAX_STEP_SIZE)
                    {
                        interpolatedLines.Add(new Line(newX, newY, lines[i + 1].On));
                        break;
                    }
                    interpolatedLines.Add(new Line(x, y, lines[i + 1].On));
                }
            }
            Lines = interpolatedLines.ToArray();
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

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => "(" + X + ", " + Y + ") " + On;
    }
}