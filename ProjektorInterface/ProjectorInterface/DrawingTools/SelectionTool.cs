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
    public class SelectionTool : DrawingTool
    {
        public Rectangle RectObj => (Rectangle)Current;

        public Point RectPos, StartPos, LastPos;
        public List<Shape> selectedShapes = new List<Shape>();
        public double _Top, _Left;
        public bool IsDragging = false;

        public double Top
        {
            get => _Top;
            set => Canvas.SetTop(this, _Top = value);
        }

        public double Left
        {
            get => _Left;
            set => Canvas.SetLeft(this, _Left = value);
        }

        public SelectionTool() : base(new Rectangle())
        {
            RectObj.StrokeThickness = Settings.SHAPE_THICKNESS;
            RectObj.Stroke = Brushes.Black;
            RectObj.StrokeDashArray = new DoubleCollection() { 4 };
            RectObj.Fill = Brushes.Transparent;
        }


        public override void Render(Point start, Point end)
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

        public override Shape CopyShape()
        {
            Rectangle tmp = new Rectangle()
            {
                StrokeThickness = Settings.SHAPE_THICKNESS,
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection() { 4 },
                Width = RectObj.Width,
                Height = RectObj.Height,
                Fill = Brushes.Transparent
            };
            tmp.Width = RectObj.Width;
            tmp.Height = RectObj.Height;

            Canvas.SetLeft(tmp, RectPos.X);
            Canvas.SetTop(tmp, RectPos.Y);
            return tmp;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            IsDragging = true;
            StartPos = e.GetPosition((Canvas)Parent);
            LastPos = StartPos;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && IsDragging)
            {
                Point currentPos = e.GetPosition((Canvas)Parent);
                Vector diff = Point.Subtract(LastPos, currentPos);
                LastPos = currentPos;

                Left -= diff.X;
                Top -= diff.Y;
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                IsDragging = false;
                Point currentPos = e.GetPosition((Canvas)Parent);
                if (Point.Subtract(currentPos, StartPos).LengthSquared > 5)
                    e.Handled = true;
            }
        }
    }
}
