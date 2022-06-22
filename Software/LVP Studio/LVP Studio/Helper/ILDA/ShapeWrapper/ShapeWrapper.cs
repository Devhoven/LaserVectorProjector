using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = ProjectorInterface.GalvoInterface.Point;

namespace LVP_Studio.Helper
{
    abstract class ShapeWrapper : Shape
    {
        // The shape
        protected Shape Shape;

        public Point StartLine;
        public Point EndLine;

        // From the "Shape"-class
        protected override Geometry DefiningGeometry => throw new NotImplementedException();

        protected ShapeWrapper(Shape shape)
        {
            Shape = shape;
            (StartLine, EndLine) = CalcEnds();
        }

        // Calculates the start and end point
        protected abstract (Point, Point) CalcEnds();

        public abstract void AddPoints(Action<double, double, bool> addPoint);
    }
}
