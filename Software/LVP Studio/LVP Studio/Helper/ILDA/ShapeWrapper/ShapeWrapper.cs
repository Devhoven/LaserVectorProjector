using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Line = ProjectorInterface.GalvoInterface.Line;

namespace LVP_Studio.Helper
{
    abstract class ShapeWrapper : Shape
    {
        // The shape
        protected Shape Shape;

        public Line StartLine;
        public Line EndLine;

        // Returns the total length of the shape
        public readonly double Length;

        // From the "Shape"-class
        protected override Geometry DefiningGeometry => throw new NotImplementedException();

        public ShapeWrapper(Shape shape)
        {
            Shape = shape;
            (StartLine, EndLine) = CalcEnds();
            Length = CalcLength();
        }

        // Calculates the start and end point
        protected abstract (Line, Line) CalcEnds();

        protected abstract double CalcLength();

        public abstract void AddPoints(Action<double, double, bool> addLine);
    }
}
