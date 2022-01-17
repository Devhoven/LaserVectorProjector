namespace GalvoInteface
{
    // Currently just a mockup class which is later going to contain an array of vectors which define the lines
    // It is going to be responsible for the normalization of the data and interpolation between the lines, if they are too far apart
    class VectorizedImage
    {
        const short MAX_VALUE = 4100;
        const short MAX_STEP_SIZE = 100;

        public Line[] Lines;

        public VectorizedImage(params PointF[] points)
        {
            List<Line> lines = new List<Line>();
            short oldX, oldY;
            short newX, newY;
            short diffX, diffY;
            float length;
            float xProp, yProp;
            float mult;
            short x, y;
            for (int i = 0; i < points.Length; i++)
            {
                oldX = (short)(points[i].X * MAX_VALUE);
                oldY = (short)(points[i].Y * MAX_VALUE);
                newX = (short)(points[(i + 1) % points.Length].X * MAX_VALUE);
                newY = (short)(points[(i + 1) % points.Length].Y * MAX_VALUE);

                diffX = (short)(newX - oldX);
                diffY = (short)(newY - oldY);

                length = MathF.Abs(diffX) + Math.Abs(diffY);

                xProp = diffX / length;
                yProp = diffY / length;

                mult = 1 / MathF.Abs(MathF.Abs(xProp) > MathF.Abs(yProp) ? xProp : yProp);
                xProp *= mult;
                yProp *= mult;

                x = oldX;
                y = oldY;

                if (i == 0)
                    lines.Add(new Line(x, y, 0, false));

                // Muss man noch ersetzen 
                while (true)
                {
                    x += (short)(MAX_STEP_SIZE * xProp);
                    y += (short)(MAX_STEP_SIZE * yProp);
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

    struct PointF
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
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
            return "(" + X + ", " + Y + ")"; //+ " Length: " + MathF.Round(MathF.Sqrt(X * X + Y * Y), 0);
        }
    }
}