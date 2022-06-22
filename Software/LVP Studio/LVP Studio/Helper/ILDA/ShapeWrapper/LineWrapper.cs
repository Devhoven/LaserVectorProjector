using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = ProjectorInterface.GalvoInterface.Point;
using WPFLine = System.Windows.Shapes.Line;

namespace LVP_Studio.Helper
{
    class LineWrapper : ShapeWrapper
    {
        // ?? null! is for resolving a stupid null warning
        new WPFLine Shape => base.Shape as WPFLine ?? null!;

        public LineWrapper(WPFLine shape) : base(shape)
        { }

        protected override (Point, Point) CalcEnds()
            => (new Point(Shape.X1, Shape.Y1, true), new Point(Shape.X2, Shape.Y2, true));

        public override void AddPoints(Action<double, double, bool> addPoint)
        {
            addPoint(StartLine.X, StartLine.Y, false);

            addPoint(EndLine.X, EndLine.Y, true);
        }
    }
}
