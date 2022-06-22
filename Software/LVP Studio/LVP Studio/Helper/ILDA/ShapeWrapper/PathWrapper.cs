using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = ProjectorInterface.GalvoInterface.Point;

namespace LVP_Studio.Helper
{
    class PathWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Path Shape => base.Shape as Path ?? null!;

        public PathWrapper(Path shape) : base(shape)
        { }

        protected override (Point, Point) CalcEnds()
        {
            GeometryCollection lineSegments = ((GeometryGroup)Shape.Data).Children;
            LineGeometry firstLine = (LineGeometry)lineSegments[0];
            LineGeometry endLine = (LineGeometry)lineSegments[lineSegments.Count - 1];

            return (new Point(firstLine.StartPoint.X, firstLine.StartPoint.Y, true), 
                    new Point(endLine.StartPoint.X, endLine.StartPoint.Y, true));
        }

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            // Retreiving all the points from the path and saving them in an point array
            PathFigureCollection flattenedPath = Shape.Data.GetFlattenedPathGeometry().Figures;
            Point[] pathPoints = new Point[flattenedPath.Count];

            System.Windows.Point currentPoint;
            for (int i = 0; i < flattenedPath.Count; i++) 
            {
                currentPoint = flattenedPath[i].StartPoint;
                pathPoints[i] = new Point(currentPoint.X, currentPoint.Y, true);
            }
            // The first point has to be off
            pathPoints[0].On = false;

            RdpLineSimplification.Execute(pathPoints, addPoint);
        }
    }
}
