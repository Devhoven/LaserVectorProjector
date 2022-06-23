using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = ProjectorInterface.GalvoInterface.Point;

namespace LVP_Studio.Helper
{
    class PathWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Path Shape => base.Shape as Path ?? null!;

        // The Canvas.GetLeft and GetTop Offset
        double XOffset;
        double YOffset;

        public PathWrapper(Path shape) : base(shape)
        {
            XOffset = Canvas.GetLeft(Shape);
            YOffset = Canvas.GetTop(Shape);
        }

        protected override (Point, Point) CalcEnds()
        {
            GeometryCollection lineSegments = ((GeometryGroup)Shape.Data).Children;
            LineGeometry firstLine = (LineGeometry)lineSegments[0];
            LineGeometry endLine = (LineGeometry)lineSegments[lineSegments.Count - 1];

            return (new Point(firstLine.StartPoint.X + XOffset, firstLine.StartPoint.Y + YOffset, true), 
                    new Point(endLine.StartPoint.X + XOffset, endLine.StartPoint.Y + YOffset, true));
        }

        // Returns the shortest distance to the given point
        // If the start point does not have the shortest distance to the point, the end point is going to be switched with the start point
        // and the path is going to be built up in "reverse"
        public override double GetShortestDistance(Point p)
        {
            double distToStart = Point.GetDistance(StartPoint, p);
            double distToEnd = Point.GetDistance(EndPoint, p);

            if (distToEnd < distToStart)
            {
                (StartPoint, EndPoint) = (EndPoint, StartPoint);
                return distToEnd;
            }

            return distToStart;
        }

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            // Retreiving all the points from the path and saving them in an point array
            PathFigureCollection flattenedPath = Shape.Data.GetFlattenedPathGeometry().Figures;
            Point[] pathPoints = new Point[flattenedPath.Count];

            System.Windows.Point currentPoint = flattenedPath[0].StartPoint;

            // Checking if the start point is still the first point on the path
            // If not, the other end had a shorter distance to the previous point
            // If that is the case, this path has to be built up in reverse
            if (StartPoint.X == (short)currentPoint.X + XOffset && StartPoint.Y == (short)currentPoint.Y + YOffset)
            {
                for (int i = 0; i < flattenedPath.Count; i++)
                {
                    currentPoint = flattenedPath[i].StartPoint;
                    pathPoints[i] = new Point(currentPoint.X + XOffset, currentPoint.Y + YOffset, true);
                }
            }
            else
            {
                for (int i = flattenedPath.Count - 1, j = 0; i >= 0; i--, j++)
                {
                    currentPoint = flattenedPath[i].StartPoint;
                    pathPoints[j] = new Point(currentPoint.X + XOffset, currentPoint.Y + YOffset, true);
                }
            }
            // The first point has to be off
            pathPoints[0].On = false;

            // Simplifies the path and adds the new simpler points to the result list
            RdpLineSimplification.Execute(pathPoints, addPoint);
        }
    }
}
