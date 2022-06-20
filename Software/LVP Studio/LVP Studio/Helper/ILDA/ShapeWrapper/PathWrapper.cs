using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Line = ProjectorInterface.GalvoInterface.Line;

namespace LVP_Studio.Helper
{
    class PathWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new Path Shape => base.Shape as Path ?? null!;

        public PathWrapper(Path shape) : base(shape)
        { }

        protected override (Line, Line) CalcEnds()
        {
            GeometryCollection lineSegments = ((GeometryGroup)Shape.Data).Children;
            LineGeometry firstLine = (LineGeometry)lineSegments[0];
            LineGeometry endLine = (LineGeometry)lineSegments[lineSegments.Count - 1];

            return (new Line(firstLine.StartPoint.X, firstLine.StartPoint.Y, true), 
                    new Line(endLine.StartPoint.X, endLine.StartPoint.Y, true));
        }

        protected override double CalcLength()
        {
            double length = 0;

            PathFigureCollection flattenedPath = Shape.Data.GetFlattenedPathGeometry().Figures;

            Point lastPoint = flattenedPath[0].StartPoint;
            for (int i = 1; i < flattenedPath.Count; i++)
            {
                Point current = flattenedPath[i].StartPoint;
                length += Point.Subtract(current, lastPoint).Length;
                lastPoint = current;
            }

            return length;
        }

        public override void AddPoints(Action<double, double, bool> addLine)
        {
            PathFigureCollection flattenedPath = Shape.Data.GetFlattenedPathGeometry().Figures;

            Point currentPoint = flattenedPath[0].StartPoint;
            addLine(currentPoint.X, currentPoint.Y, false);
            for (int i = 1; i < flattenedPath.Count; i++)
            {
                currentPoint = flattenedPath[i].StartPoint;
                addLine(currentPoint.X, currentPoint.Y, true);
            }
        }
    }
}
