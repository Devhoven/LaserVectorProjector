using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectorInterface.DrawingTools
{
    // Allows the user to select Shapes
    public class SelectionTool : Shape
    {
        Rectangle RectObj = new Rectangle();

        Point RectPos;
        List<Line> selectedShapes = new List<Line>();

        protected override Geometry DefiningGeometry
        {
            get { return new RectangleGeometry(); }
        }

        public SelectionTool() : base()
        {
            RectObj.StrokeThickness = Settings.SHAPE_THICKNESS;
            RectObj.Stroke = Brushes.Black;
            RectObj.StrokeDashArray = new DoubleCollection() { 4 };
        }

        public void Render(Point start, Point end)
        {
            Point topLeft = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point bottomRight = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double size = Math.Max(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                RectObj.Width = size;
                RectObj.Height = size;
            }
            else
            {
                RectObj.Width = bottomRight.X - topLeft.X;
                RectObj.Height = bottomRight.Y - topLeft.Y;
            }

            Canvas.SetLeft(this, RectPos.X = topLeft.X);
            Canvas.SetTop(this, RectPos.Y = topLeft.Y);
        }

        public void GetSelectedShapes()
        {
            selectedShapes.Clear();

        }
    }
}
