using ProjectorInterface.DrawingCommands;
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
    public class SelectionRectangle : Shape
    {

        public Point RectPos, StartPos, LastPos, MovePos;
        public List<Shape> selectedShapes = new List<Shape>();
        public double _Top, _Left;
        public bool isSelecting = false;


        protected override Geometry DefiningGeometry
        {
            get { return new RectangleGeometry(new Rect() { Width = this.Width, Height = this.Height }); }
        }

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

        public SelectionRectangle() : base()
        {
            StrokeThickness = Settings.SHAPE_THICKNESS;
            Stroke = Brushes.Black;
            StrokeDashArray = new DoubleCollection() { 4 };
            Fill = Brushes.Transparent;
        }

        public void DeselectAll()
        {
            foreach (Shape shape in selectedShapes)
            {
                shape.Stroke = Brushes.Red;
            }
            selectedShapes.Clear();
        }

        public void DeleteShapes()
        {
            foreach (Shape s in selectedShapes)
            {
                ((Canvas)Parent).Children.Remove(s);
            }
        }

        public void SelectShape(Shape s)
        {
            if (selectedShapes.Contains(s) && s is not SelectionRectangle)
            {
                selectedShapes.Remove(s);
                s.Stroke = Brushes.Red;
            }
            else if (!selectedShapes.Contains(s) && s is not SelectionRectangle)
            {
                selectedShapes.Add(s);
                s.Stroke = Brushes.Blue;
            }
        }

        // Selects all Shapes within the selection-rectangle
        public void ApplySelection()
        {
            Rect dragRect = new Rect(RectPos.X, RectPos.Y, Width, Height);


            foreach (object child in ((Canvas)Parent).Children)
            {
                if (child is Line line)
                {
                    if (dragRect.Contains(line.X1, line.Y1) || dragRect.Contains(line.X2, line.Y2))
                    {
                        selectedShapes.Add(line);
                        line.Stroke = Brushes.Blue;
                    }
                }
                else if (child is Rectangle rec)
                {
                    double recLeft = Canvas.GetLeft(rec);
                    double recTop = Canvas.GetTop(rec);

                    if (dragRect.Contains(recLeft, recTop) || dragRect.Contains(recLeft + rec.Width, recTop) ||
                        dragRect.Contains(recLeft + rec.Width, recTop + rec.Height) || dragRect.Contains(recLeft, recTop + rec.Height))
                    {
                        selectedShapes.Add(rec);
                        rec.Stroke = Brushes.Blue;
                    }
                }
                else if (child is Ellipse ell)
                {
                    double ellLeft = Canvas.GetLeft(ell);
                    double ellTop = Canvas.GetTop(ell);

                    if (dragRect.Contains(ellLeft + ell.Width / 2, ellTop) || dragRect.Contains(ellLeft + ell.Width, ellTop + ell.Height / 2) ||
                        dragRect.Contains(ellLeft + ell.Width / 2, ellTop + ell.Height) || dragRect.Contains(ellLeft, ellTop + ell.Height / 2))
                    {
                        selectedShapes.Add(ell);
                        ell.Stroke = Brushes.Blue;
                    }
                }
                else if (child is Path path)
                {
                    GeometryCollection lineSegments = ((GeometryGroup)path.Data).Children;
                    foreach (LineGeometry lg in lineSegments)
                    {
                        if (dragRect.Contains(lg.StartPoint.X, lg.StartPoint.Y) || dragRect.Contains(lg.EndPoint.X, lg.EndPoint.Y))
                        {
                            selectedShapes.Add(path);
                            path.Stroke = Brushes.Blue;
                            break;
                        }
                    }

                }
                else
                    continue;
            }
        }

        public void Render(Point start, Point end)
        {
            Point topLeft = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point bottomRight = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double size = Math.Max(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                Width = size;
                Height = size;
            }
            else
            {
                Width = bottomRight.X - topLeft.X;
                Height = bottomRight.Y - topLeft.Y;
            }

            Canvas.SetLeft(this, RectPos.X = topLeft.X);
            Canvas.SetTop(this, RectPos.Y = topLeft.Y);
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Deleting Selected Shapes
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Width = 0;
                Height = 0;

            }
            // Moving selected Shapes
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                MovePos = e.GetPosition((Canvas)Parent);
            }

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Move Selectes Shapes
            if (e.RightButton == MouseButtonState.Pressed && isSelecting)
            {
                // Calculating position and 
                Point currentPos = e.GetPosition((Canvas)Parent);
                Vector diff = Point.Subtract(MovePos, currentPos);
                MovePos = currentPos;
                GeometryCollection lineSegments;

                Canvas.SetLeft(this, RectPos.X -= diff.X);
                Canvas.SetTop(this, RectPos.Y -= diff.Y);

                foreach (Shape s in selectedShapes)
                {
                    if (s is Line line)
                    {
                        line.X1 -= diff.X;
                        line.X2 -= diff.X;
                        line.Y1 -= diff.Y;
                        line.Y2 -= diff.Y;
                    }
                    else if (s is Rectangle rec)
                    {
                        Canvas.SetLeft(rec, Canvas.GetLeft(rec) - diff.X);
                        Canvas.SetTop(rec, Canvas.GetTop(rec) - diff.Y);
                    }
                    else if (s is Ellipse ell)
                    {
                        Canvas.SetLeft(ell, Canvas.GetLeft(ell) - diff.X);
                        Canvas.SetTop(ell, Canvas.GetTop(ell) - diff.Y);
                    }
                    else if (s is Path path)
                    {
                        
                        lineSegments = ((GeometryGroup)path.Data).Children;
                       
                        foreach (LineGeometry lg in lineSegments)
                        {
                            lg.StartPoint = new Point(lg.StartPoint.X - diff.X, lg.StartPoint.Y - diff.Y);
                            lg.EndPoint = new Point(lg.EndPoint.X - diff.X, lg.EndPoint.Y - diff.Y);
                        }
                    }
                    else
                        continue;
                }
            }

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                ApplySelection();
            else if (e.ChangedButton == MouseButton.Right)
                e.Handled = true;
        }
    }
}
