using LVP_Studio.DrawingCommands;
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
        public event EventHandler? SelectionChanged;
        public HashSet<Shape> SelectedShapes = new HashSet<Shape>();
        public Point StartPos, LastPos;

        Point StartMovePos, MovePos;
        double _Top, _Left;
        bool _Selecting = false;

        CommandHistory Commands;

        public bool IsSelecting
        {
            get => _Selecting;
            set
            {
                _Selecting = value;
                if (IsSelecting && SelectionChanged != null)
                    SelectionChanged(this, EventArgs.Empty);
            }
        }

        protected override Geometry DefiningGeometry
        {
            get => new RectangleGeometry(new Rect() 
            { 
                Width = this.Width, 
                Height = this.Height 
            });
        }

        public double Top
        {
            get => _Top;
            set
            {
                _Top = value;
                Canvas.SetTop(this, _Top);
            }
        }

        public double Left
        {
            get => _Left;
            set
            {
                _Left = value;
                Canvas.SetLeft(this, _Left);
            }
        }

        public SelectionRectangle(CommandHistory commands)
        {
            StrokeThickness = Settings.SHAPE_THICKNESS;
            Stroke = Brushes.Black;
            StrokeDashArray = new DoubleCollection() { 4 };
            Fill = Brushes.Transparent;
            Commands = commands;
        }

        // Deselects all Shapes
        public void DeselectAll()
        {
            foreach (Shape shape in SelectedShapes)
                shape.Stroke = Brushes.Red;

            Width = 0;
            Height = 0;
            SelectedShapes.Clear();
        }

        // Selects all Shapes in the current Canvas
        public void SelectAll()
        {
            IsSelecting = true;
            Width = ((Canvas)Parent).ActualWidth;
            Height = ((Canvas)Parent).ActualHeight;
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            ApplySelection();
        }

        // Ctrl + LeftClick Method to select/deselect single Shapes
        public void SelectShape(Shape s)
        {
            if (SelectedShapes.Contains(s) && s is not SelectionRectangle)
            {
                SelectedShapes.Remove(s);
                s.Stroke = Brushes.Red;
            }
            else if (!SelectedShapes.Contains(s) && s is not SelectionRectangle)
            {
                SelectedShapes.Add(s);
                s.Stroke = Brushes.Blue;
            }
        }

        // Selects all Shapes within the selection-rectangle
        public void ApplySelection()
        {
            Rect dragRect = new Rect(Left, Top, Width, Height);

            // Checking for each Shape in canvas, if it is contained in the selection rectangle
            foreach (UIElement child in ((Canvas)Parent).Children)
            {
                if (child is not Shape)
                    continue;

                double childLeft = Canvas.GetLeft(child);
                double childTop = Canvas.GetTop(child);

                if (child is Line line)
                {
                    if (dragRect.Contains(childLeft + line.X1, childTop + line.Y1) || 
                        dragRect.Contains(childLeft + line.X2, childTop + line.Y2))
                    {
                        SelectedShapes.Add(line);
                        line.Stroke = Brushes.Blue;
                    }
                }
                else if (child is Rectangle rec)
                {
                    if (dragRect.Contains(childLeft, childTop) || 
                        dragRect.Contains(childLeft + rec.Width, childTop) ||
                        dragRect.Contains(childLeft + rec.Width, childTop + rec.Height) || 
                        dragRect.Contains(childLeft, childTop + rec.Height))
                    {
                        SelectedShapes.Add(rec);
                        rec.Stroke = Brushes.Blue;
                    }
                }
                else if (child is Ellipse ell)
                {
                    if (dragRect.Contains(childLeft + ell.Width / 2, childTop) || 
                        dragRect.Contains(childLeft + ell.Width, childTop + ell.Height / 2) ||
                        dragRect.Contains(childLeft + ell.Width / 2, childTop + ell.Height) || 
                        dragRect.Contains(childLeft, childTop + ell.Height / 2))
                    {
                        SelectedShapes.Add(ell);
                        ell.Stroke = Brushes.Blue;
                    }
                }
                else if (child is Path path)
                {
                    GeometryCollection lineSegments = ((GeometryGroup)path.Data).Children;
                    foreach (LineGeometry lg in lineSegments)
                    {
                        // if one single line segment is selected, select the whole path
                        if (dragRect.Contains(childLeft + lg.StartPoint.X, childTop + lg.StartPoint.Y) || 
                            dragRect.Contains(childLeft + lg.EndPoint.X, childTop + lg.EndPoint.Y))
                        {
                            SelectedShapes.Add(path);
                            path.Stroke = Brushes.Blue;
                            break;
                        }
                    }
                }
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

            Left = topLeft.X;
            Top = topLeft.Y;
        }

        // Saving Mouseposition for movement of selected Shapes
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Moving selected Shapes
            if (e.RightButton == MouseButtonState.Pressed)
            {
                MovePos = e.GetPosition((Canvas)Parent);
                StartMovePos = MovePos;
            }
        }

        // Move Selectes Shapes
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && IsSelecting)
            {
                // Getting current position and amount of movement
                Point currentPos = e.GetPosition((Canvas)Parent);
                Vector moveDiff = Point.Subtract(MovePos, currentPos);
                MovePos = currentPos;

                Left -= moveDiff.X;
                Top -= moveDiff.Y;

                foreach (Shape s in SelectedShapes)
                {
                    Canvas.SetLeft(s, Canvas.GetLeft(s) - moveDiff.X);
                    Canvas.SetTop(s, Canvas.GetTop(s) - moveDiff.Y);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                FinishMove(e.GetPosition((Canvas)Parent));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                MovePos = e.GetPosition((Canvas)Parent);
                StartMovePos = MovePos;
            }
        }

        // Excecutes the MoveSelectionCommand
        void FinishMove(Point mousePos)
        {
            Vector moveDiff = Point.Subtract(StartMovePos, mousePos);

            // The shapes are going to be moved again in the Command.Execute method, which is important for the REDO and UNDO functionality
            // This is why the shapes have to be moved back into the position before the move
            MoveSelectionCommand moveCom = new MoveSelectionCommand(this, Left, Top, Width, Height, moveDiff);
            moveCom.Undo();
            Left += moveDiff.X;
            Top += moveDiff.Y;

            Commands.Execute(moveCom);
        }
    }
}
