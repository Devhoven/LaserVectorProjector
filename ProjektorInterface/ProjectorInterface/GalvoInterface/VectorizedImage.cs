using System;
using System.Collections.Generic;
using static ProjectorInterface.Settings;

namespace ProjectorInterface.GalvoInterface
{
    // Currently just a mockup class which is later going to contain an array of vectors which define the lines
    // It is going to be responsible for the normalization of the data and interpolation between the lines, if they are too far apart
    class VectorizedImage
    {
        // Array of all the finished lines
        public readonly Line[] Lines;

        public VectorizedImage(params PointF[] points)
        {
            List<Line> lines = new List<Line>();
            short x, y;
            short newX, newY;
            short diffX, diffY;
            float dist;
            float xRatio, yRatio;
            float mult;
            for (int i = 0; i < points.Length; i++)
            {
                // Retreiving the start points
                x = (short)(points[i].X * MAX_VOLTAGE);
                y = (short)(points[i].Y * MAX_VOLTAGE);
                // Retreiving the end points of this line segment
                // Loops around, so that the last points are connected to the first
                newX = (short)(points[(i + 1) % points.Length].X * MAX_VOLTAGE);
                newY = (short)(points[(i + 1) % points.Length].Y * MAX_VOLTAGE);

                // Calculating the difference between the old and new coordinates
                diffX = (short)(newX - x);
                diffY = (short)(newY - y);

                // Calculating the manhattan distance
                dist = MathF.Abs(diffX) + Math.Abs(diffY);

                // Ratio between the difference and the manhattan distance
                xRatio = diffX / dist;
                yRatio = diffY / dist;

                // TODO: SHOULD CHECK IF TWO POINTS ARE THE SAME

                // Scaling one of the ratios up to 1, (the other one accordingly) so that the galvos behave the fastest they possibly can
                mult = 1 / MathF.Abs(MathF.Abs(xRatio) > MathF.Abs(yRatio) ? xRatio : yRatio);
                xRatio *= mult;
                yRatio *= mult;

                // Otherwise the first point won't be added
                if (i == 0)
                    lines.Add(new Line(x, y, 0, false));

                // Traverses from one point to another until the distance is too short and adds the points on the way
                while (true)
                {
                    x += (short)(MAX_STEP_SIZE * xRatio);
                    y += (short)(MAX_STEP_SIZE * yRatio);
                    if (Math.Abs(x - newX) < MAX_STEP_SIZE && Math.Abs(y - newY) < MAX_STEP_SIZE)
                    {
                        lines.Add(new Line(newX, newY, 0, true));
                        break;
                    }
                    lines.Add(new Line(x, y, 0, true));
                }
            }
            Lines = lines.ToArray();
        }
    }

    // struct, which holds normalized coordinates between 0 and 1
    // and, if the line which is formed between this point and the one after it, should have the laser enabled
    struct PointF
    {
        public float X;
        public float Y;
        public bool On;

        public PointF(short x, short y, bool on = false)
        {
            // Normalizing the x and y short coordinates between 0 and 1
            X = (x + short.MaxValue) / (float)(short.MaxValue * 2);
            y *= -1;
            Y = (y + short.MaxValue) / (float)(short.MaxValue * 2);
            On = on;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }

    // This line - struct contains the x and y coordinates (normalized between 0 and MaxValue) and how many microseconds the arduino should wait 
    // It also holds, if the laser should be turned on or off
    struct Line
    {
        public short X;
        public short Y;
        public byte Delay;
        public bool On;

        public Line(int x, int y, byte delay, bool on)
        {
            X = (short)x;
            Y = (short)y;
            Delay = delay;
            On = on;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}