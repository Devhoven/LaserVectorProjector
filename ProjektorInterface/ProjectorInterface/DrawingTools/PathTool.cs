using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectorInterface.DrawingTools
{
    // Allows the user to draw a path with the mouse 
    class PathTool : DrawingTool
    {
        Path PathObj => (Path)Current;

        GeometryGroup Geometry;

        Point LastPoint;

        public PathTool() : base(new Path())
        {
            Geometry = new GeometryGroup();
            PathObj.Data = Geometry;

            Current.Width = double.MaxValue;
            Current.Height = double.MaxValue;
        }

        public override void Render(Point start, Point end)
        {
            if (LastPoint.X == 0 && LastPoint.Y == 0)
            {
                LastPoint = start;
                Geometry.Children.Add(new LineGeometry(LastPoint, end));
            }
            else
            {
                Geometry.Children.Add(new LineGeometry(LastPoint, end));
                LastPoint = end;
            }
        }

        public override Shape CopyShape()
        {
            LastPoint = new Point();

            Path tmp = new Path()
            {
                StrokeThickness = Settings.SHAPE_THICKNESS,
                Stroke = Settings.SHAPE_COLOR,
                Data = Geometry.Clone(),
                Width = double.MaxValue,
                Height = double.MaxValue
            };

            Geometry = new GeometryGroup();
            PathObj.Data = Geometry;

            return tmp;
        }
    }
}
