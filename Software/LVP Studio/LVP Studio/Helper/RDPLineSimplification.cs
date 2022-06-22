using ProjectorInterface.GalvoInterface;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVP_Studio.Helper
{
    static class RdpLineSimplification
    {
        static List<Point> RDPResult = new List<Point>();

        public static void Execute(Point[] pathPoints, Action<double, double, bool> addPoint)
        {
            RDPResult.Add(pathPoints[0]);
            RdpLineSimplificationRec(pathPoints, 0, pathPoints.Length - 2);
            RDPResult.Add(pathPoints[pathPoints.Length - 1]);

            foreach (Point p in RDPResult)
                addPoint(p.X, p.Y, p.On);

            RDPResult.Clear();
        }

        static void RdpLineSimplificationRec(Point[] pathPoints, int startIndex, int endIndex)
        {
            int furthestIndex = FindFurthest(pathPoints, startIndex, endIndex);

            if (furthestIndex > startIndex)
            {
                RdpLineSimplificationRec(pathPoints, startIndex, furthestIndex);

                RDPResult.Add(pathPoints[furthestIndex]);

                RdpLineSimplificationRec(pathPoints, furthestIndex, endIndex);
            }
        }

        static int FindFurthest(Point[] pathPoints, int startIndex, int endIndex)
        {
            double currentDist;
            double maxDist = 0;
            int maxDistIndex = -1;

            Point lineStart = pathPoints[startIndex];
            Point lineEnd = pathPoints[endIndex];
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                currentDist = LineDistance(lineStart, lineEnd, pathPoints[i]);
                if (currentDist > maxDist)
                {
                    maxDist = currentDist;
                    maxDistIndex = i;
                }
            }

            if (maxDist < Settings.RDP_EPSILON)
                return -1;
            return maxDistIndex;
        }

        // Calculates the shortest distance between the line (formed in between lineStart and lineEnd) and p
        static double LineDistance(Point lineStart, Point lineEnd, Point p)
            => Math.Abs((lineEnd.X - lineStart.X) * (lineStart.Y - p.Y) - (lineStart.X - p.X) * (lineEnd.Y - lineStart.Y)) /
                   Point.GetDistance(lineStart, lineEnd);
    }

}
