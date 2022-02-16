using ProjectorInterface.Commands;
using ProjectorInterface.DrawingTools;
using ProjectorInterface.Helper;
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

            Commands = (CommandHistory)GetValue(MainWindow.CommandHistoryProperty);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Getting the mouse pos relative to the canvas
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StartMousePos = e.GetPosition(this);
                Children.Add(CurrentTool);
                CurrentTool.Render(StartMousePos, StartMousePos);
            }
            
            // If the user middleclicked on one of the shapes, it is going to be deleted
            if (e.MiddleButton == MouseButtonState.Pressed && e.OriginalSource is Shape)
            {
                Commands.Execute(new EraseShapeCommand((Shape)e.OriginalSource));
                return;
            }
                
            Keyboard.Focus(this);
        }

        // As long as the mouse moves and the left mouse button is pressed, the currently selected tool updates its visuals
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !Keyboard.IsKeyDown(Key.LeftAlt))
                CurrentTool.Render(StartMousePos, e.GetPosition(this));
        }

        // If the user releases one of the mouse buttons the tool visuals are going to be removed 
        // and potentially a new shape gets added to the canvas
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !Keyboard.IsKeyDown(Key.LeftAlt))
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
    }
}
