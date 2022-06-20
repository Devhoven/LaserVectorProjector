using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Line = ProjectorInterface.GalvoInterface.Line;
using WPFLine = System.Windows.Shapes.Line;

namespace LVP_Studio.Helper
{
    class LineWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new WPFLine Shape => base.Shape as WPFLine ?? null!;

        public LineWrapper(WPFLine shape) : base(shape)
        { }

        protected override (Line, Line) CalcEnds()
            => (new Line(Shape.X1, Shape.Y1, true), new Line(Shape.X2, Shape.Y2, true));

        protected override double CalcLength()
            => Line.GetDistance(StartLine, EndLine);

        public override void AddPoints(Action<double, double, bool> addLine)
        {
            addLine(StartLine.X, StartLine.Y, false);

            addLine(EndLine.X, EndLine.Y, true);
        }
    }
}
