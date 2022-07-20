using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = LvpStudio.GalvoInterface.Point;

namespace LVP_Studio.Helper
{
    abstract class ShapeWrapper : Shape
    {
        // The shape
        protected Shape Shape;

        public Point StartPoint;
        public Point EndPoint;

        // From the "Shape"-class
        protected override Geometry DefiningGeometry => throw new NotImplementedException();

        protected ShapeWrapper(Shape shape)
        {
            Shape = shape;
            (StartPoint, EndPoint) = CalcEnds();
        }

        // Calculates the start and end point
        protected abstract (Point, Point) CalcEnds();

        // Returns the shortest distance to the given point
        public abstract double GetShortestDistance(Point p);

        // Adds the points of the shape to the result list, with the addPoint - method
        public abstract void AddPoints(Action<double, double, bool> addPoint);
    }
}
