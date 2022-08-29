using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LvpStudio.GalvoInterface
{

    // This point - struct contains the x and y coordinates (normalized between 0 and MaxValue) and if the laser should be turned on or off
    struct Point
    {
        public short X;
        public short Y;
        public bool On;

        public Point(int x, int y, bool on = true)
        {
            X = (short)x;
            Y = (short)y;
            On = on;
        }

        public Point(double x, double y, bool on = true)
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

        public static bool operator ==(Point v1, Point v2)
            => v1.X == v2.X && v1.Y == v2.Y;

        public static bool operator !=(Point v1, Point v2)
            => !(v1 == v2);

        public static Vector operator -(Point v1, Point v2)
            => new Vector(v1.X - v2.X, v1.Y - v2.Y);

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
