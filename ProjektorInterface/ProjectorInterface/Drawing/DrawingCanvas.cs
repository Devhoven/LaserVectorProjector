using ProjectorInterface.DrawingCommands;
using ProjectorInterface.DrawingTools;
using ProjectorInterface.Helper;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectorInterface
{
    // A canvas the user is able to draw on
    public class DrawingCanvas : Canvas
    {
        // Gets set when the user clicks on the canvas
        Point StartMousePos;

        // Holds all of the actions the user did, like drawing or deleting something
        public CommandHistory Commands { get; set; }
        CommandDisplay CommandDisplay;

        // Holds the current tool one is drawing with
        public DrawingTool CurrentTool;

        // Background image of the canvas
        // Can be controlled, so the user is able to trace the outlines
        public MoveableImage BackgroundImg;

        public DrawingCanvas()
        {
            CurrentTool = new LineTool();

            BackgroundImg = new MoveableImage();
            Children.Add(BackgroundImg);

            Background = Brushes.White;
            ClipToBounds = true;
            Focusable = true;

            CanvasCommand.Parent = this;

            Commands = new CommandHistory();
            CommandDisplay = new CommandDisplay(Commands);
            Children.Add(CommandDisplay);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Getting the mouse pos relative to the canvas
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (CurrentTool is SelectionTool curr)
                {
                    if (Children[Children.Count - 1] is Rectangle r)
                    {
                        foreach (Shape s in curr.selectedShapes)
                        {
                            s.Stroke = Brushes.Red;
                        }
                        Children.RemoveAt(Children.Count - 1);
                    }

                    // Ctrl + Click Adds Shape to selected Shapes
                    if (e.OriginalSource is Shape shape && Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        if (((SelectionTool)CurrentTool).selectedShapes.Contains(shape))
                        {
                            ((SelectionTool)CurrentTool).selectedShapes.Remove(shape);
                            shape.Stroke = Brushes.Red;
                        }
                        else
                        {
                            ((SelectionTool)CurrentTool).selectedShapes.Add(shape);
                            shape.Stroke = Brushes.Blue;
                        }
                        
                    }
                    curr.StartPos = e.GetPosition(this);
                    curr.LastPos = curr.StartPos;
                }

                StartMousePos = e.GetPosition(this);
                Children.Add(CurrentTool);
                CurrentTool.Render(StartMousePos, StartMousePos);
            }// If the user middleclicked on one of the shapes, it is going to be deleted
            else if (e.MiddleButton == MouseButtonState.Pressed && e.OriginalSource is Shape origShape)
            {
                if (e.OriginalSource is Rectangle && CurrentTool is SelectionTool selec)
                {
                    foreach(Shape s in selec.selectedShapes)
                    {
                        Children.Remove(s);
                    }
                    Children.Remove(origShape);
                    selec.selectedShapes.Clear();
                }else
                    Commands.Execute(new EraseShapeCommand((Shape)e.OriginalSource));
                return;
            }
            else if(e.RightButton == MouseButtonState.Pressed && e.OriginalSource is Rectangle)
            {
                //((SelectionTool)CurrentTool).LastPos = e.GetPosition(this);
            }
                
            Keyboard.Focus(this);
        }

        // As long as the mouse moves and the left mouse button is pressed, the currently selected tool updates its visuals
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && CurrentTool is SelectionTool selec)
            {
                Point currentPos = e.GetPosition(this);
                Vector diff = Point.Subtract(selec.LastPos, currentPos);
                selec.LastPos = currentPos;

                SetLeft(Children[Children.Count - 1], -diff.X + GetLeft(Children[Children.Count - 1]));
                SetTop(Children[Children.Count - 1], -diff.Y + GetTop(Children[Children.Count - 1]));
                
                foreach(Shape s in selec.selectedShapes)
                {
                    
                    SetLeft((Shape)s, GetLeft((Shape)s) - diff.X);
                    SetTop((Shape)s, GetTop((Shape)s) - diff.Y);

                    if (s is Line line )
                    {
                        line.X1 -= diff.X;
                        line.X2 -= diff.X;
                        line.Y1 -= diff.Y;
                        line.Y2 -= diff.Y;
                    }
                    else if (s is Rectangle rec)
                    {
                        SetLeft(rec, GetLeft(rec) - diff.X);
                        SetTop(rec, GetTop(rec) - diff.Y);
                    }
                    else if (s is Ellipse ell)
                    {
                        SetLeft(ell, GetLeft(ell) + (ell.Width / 2) - diff.X);
                        SetTop(ell, GetTop(ell) + (ell.Height / 2) - diff.Y);
                    }
                    else if (s is Path path)
                    {
                        //get points of path segments
                    }
                    else
                        continue;
                }
                
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CurrentTool.Render(StartMousePos, e.GetPosition(this));
            }
        }

        // If the user releases one of the mouse buttons the tool visuals are going to be removed 
        // and potentially a new shape gets added to the canvas
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if(CurrentTool is SelectionTool seltool)
            {
                double x = GetLeft(seltool);
                double y = GetTop(seltool);
                double width = seltool.RectObj.Width;
                double height = seltool.RectObj.Height;
                Rect dragRect = new Rect(x, y, width, height);

                seltool.selectedShapes.Clear();

                foreach (object child in Children)
                {
                    if (child is Line line)
                    {
                        if (dragRect.Contains((int)line.X1, (int)line.Y1) || dragRect.Contains((int)line.X2, (int)line.Y2))
                        {
                            seltool.selectedShapes.Add(line);
                            line.Stroke = Brushes.Blue;
                        }
                    }else if(child is Rectangle rec)
                    {
                        if (dragRect.Contains(GetLeft(rec), GetTop(rec)) || 
                            dragRect.Contains(GetLeft(rec) + rec.Width, GetTop(rec)) || 
                            dragRect.Contains(GetLeft(rec) + rec.Width, GetTop(rec) + (int)rec.Height) ||
                            dragRect.Contains(GetLeft(rec) + rec.Width, GetTop(rec) + (int)rec.Height))
                        {
                            seltool.selectedShapes.Add(rec);
                            rec.Stroke = Brushes.Blue;
                        }
                    }
                    else if(child is Ellipse ell)
                    {
                        if (dragRect.Contains(GetLeft(ell) + ell.Width, GetTop(ell)) ||
                            dragRect.Contains(GetLeft(ell), GetTop(ell) + ell.Height) ||
                            dragRect.Contains(GetLeft(ell) + ell.Width*2, GetTop(ell) + ell.Height) ||
                            dragRect.Contains(GetLeft(ell) + ell.Width, GetTop(ell) + ell.Height*2))
                        {
                            seltool.selectedShapes.Add(ell);
                            ell.Stroke = Brushes.Blue;
                        }
                    }
                    else if(child is Path path)
                    {
                        //get points of path segments
                    }
                    else
                        continue;
                }
            }


            if (e.ChangedButton == MouseButton.Left)
                RemoveToolAndCopy(e.GetPosition(this));
            Keyboard.Focus(this);
     
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // If the mouse leaves the canvas, and the left mouse button was still pressed the operation is going to cancel
            // Doesn't work for some reason if you move onto the windows taskbar
            if (e.LeftButton == MouseButtonState.Pressed)
                RemoveToolAndCopy(e.GetPosition(this));
        }


        // Removes the visuals of the currently used tool from the canvas and adds the drawn shape onto it
        void RemoveToolAndCopy(Point current)
        {
            Children.Remove(CurrentTool);
            // Does not add a new shape, if the user did not move the mouse
            if (StartMousePos != current)
                Commands.Execute(new AddShapeCommand(CurrentTool.CopyShape()));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // The user can select a drawing mode with the keys 1 - 4
            if (e.Key == Key.D1)
                CurrentTool = new LineTool();
            else if (e.Key == Key.D2)
                CurrentTool = new RectTool();
            else if (e.Key == Key.D3)
                CurrentTool = new CircleTool();
            else if (e.Key == Key.D4)
                CurrentTool = new PathTool();
            else if (e.Key == Key.D5)
                CurrentTool = new SelectionTool();

            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // CTRL + Z = Undo
                if (Keyboard.IsKeyDown(Key.Z))
                    Commands.Undo();
                // CTRL + Y = Redo
                else if (Keyboard.IsKeyDown(Key.Y))
                    Commands.Redo();
            }

            // Operations for the background image
            if (e.Key == Key.Delete)
                BackgroundImg.ToggleOpacity();
            else if (e.Key == Key.PageUp)
                BackgroundImg.ZoomIn();
            else if (e.Key == Key.PageDown)
                BackgroundImg.ZoomOut();
            else if (e.Key == Key.Enter)
                BackgroundImg.ChooseImg();
            else if (e.Key == Key.Space)
                BackgroundImg.Reset();
        }

        // Clears the content of the canvas (only the lines etc.)
        public void Clear()
            => Children.RemoveRange(2, Children.Count - 1);

        public void ChooseBackgroundImg()
            => BackgroundImg.ChooseImg();
    }
}
