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
        public event EventHandler SelectionChanged;
        public Point RectPos, StartPos, LastPos, MovePos;
        public HashSet<Shape> selectedShapes = new HashSet<Shape>();
        public double _Top, _Left;
        public bool _selecting = false;
        public bool isSelecting
        {
            get => _selecting;
            set
            {
                _selecting = value;
                OnSelectionChanged();
            }
        }

        private void OnSelectionChanged()
        {
            EventHandler eh = SelectionChanged;
            if (isSelecting && eh != null)
                SelectionChanged(this, EventArgs.Empty);
        }


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

        // Deselects all Shapes
        public void DeselectAll()
        {
            foreach (Shape shape in selectedShapes)
            {
                shape.Stroke = Brushes.Red;
            }
            Width = 0;
            Height = 0;
            selectedShapes.Clear();
        }

        // Selects all Shapes in the current Canvas
        public void SelectAll()
        {
            isSelecting = true;
            Width = ((Canvas)Parent).ActualWidth;
            Height = ((Canvas)Parent).ActualHeight;
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            RectPos.X = 0;
            RectPos.Y = 0;
            ApplySelection();
        }

        // Ctrl + LeftClick Method to select/deselect single Shapes
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


            // Checking for each Shape in canvas, if it is contained in the selection rectangle
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
                        // if one single line segment is selected, select the whole path
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

        // Renders the selection rectangle
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

        // Saving Mouseposition for movement of selected Shapes
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Moving selected Shapes
            if (e.RightButton == MouseButtonState.Pressed)
                MovePos = e.GetPosition((Canvas)Parent);
        }

        // Move Selectes Shapes
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && isSelecting)
            {
                // Getting current position and amount of movement
                Point currentPos = e.GetPosition((Canvas)Parent);
                Vector diff = Point.Subtract(MovePos, currentPos);
                MovePos = currentPos;
                GeometryCollection lineSegments;

                Canvas.SetLeft(this, RectPos.X -= diff.X);
                Canvas.SetTop(this, RectPos.Y -= diff.Y);

                // Apply Movement for each selected Shape
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


    }
}
