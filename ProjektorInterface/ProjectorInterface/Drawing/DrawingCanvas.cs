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

        // Selection Rectangle to select multiple shapes
        public SelectionRectangle Selection;


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

            Selection = new SelectionRectangle();
            Selection.Visibility = Visibility.Visible;
            Children.Add(Selection);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Left click, either drawing or selecting
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Selecting Shapes
                if (Selection.isSelecting)
                {
                    // Ctrl + Click Adds single Shape to selected Shapes
                    if (e.OriginalSource is Shape shape && Keyboard.IsKeyDown(Key.LeftCtrl))
                        Selection.SelectShape(shape);
                    else // Select multiple Shapes with SelectionRectangle
                    {
                        Selection.GetStartPosition(e.GetPosition(this));
                    }
                }
                // Drawing Shapes
                else
                {
                    Selection.DeselectAll();

                    StartMousePos = e.GetPosition(this);
                    Children.Add(CurrentTool);
                    CurrentTool.Render(StartMousePos, StartMousePos);
                }
            }
            // If the user middleclicked on one of the shapes, it is going to be deleted
            else if (e.MiddleButton == MouseButtonState.Pressed && e.OriginalSource is Shape origShape)
            {
                if (origShape is SelectionRectangle)
                {
                    Commands.Execute(new EraseSelectionCommand((Shape)e.OriginalSource));
                }
                else
                {
                    Children.Remove(origShape);
                    Commands.Execute(new EraseShapeCommand((Shape)e.OriginalSource));
                }
            }

            Keyboard.Focus(this);
        }

        // As long as the mouse moves and the left mouse button is pressed, the currently selected tool updates its visuals
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Drawing with left Mouse Button
            if (e.LeftButton == MouseButtonState.Pressed && !Selection.isSelecting)
            {
                CurrentTool.Render(StartMousePos, e.GetPosition(this));
            }
            // Selecting with left Mouse Button
            else if (e.LeftButton == MouseButtonState.Pressed && Selection.isSelecting)
            {
                Selection.Render(Selection.StartPos, e.GetPosition(this));
            }
        }

        // If the user releases one of the mouse buttons the tool visuals are going to be removed 
        // and potentially a new shape gets added to the canvas
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !Selection.isSelecting)
                RemoveToolAndCopy(e.GetPosition(this));
            else if (e.ChangedButton == MouseButton.Left && Selection.isSelecting)
            {
                if (Selection.StartPos == e.GetPosition(this))
                    Selection.DeselectAll();
                
                if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                    Selection.ApplySelection();
            }
            Keyboard.Focus(this);
        }

        public void UpdateTool(DrawingTool tool)
        {
            CurrentTool = tool;
            Selection.isSelecting = false;
            Selection.DeselectAll();
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
                Selection.isSelecting = true;
            else if (e.Key == Key.Delete && Selection.isSelecting && Selection.selectedShapes.Count != 0)
                Commands.Execute(new EraseSelectionCommand(Selection));
            else if (e.Key == Key.Escape)
                Selection.DeselectAll();

            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // CTRL + Z = Undo
                if (Keyboard.IsKeyDown(Key.Z))
                    Commands.Undo();
                // CTRL + Y = Redo
                else if (Keyboard.IsKeyDown(Key.Y))
                    Commands.Redo();
                else if (Keyboard.IsKeyDown(Key.A))
                    Selection.SelectAll();
            }

            // Operations for the background image
            if (e.Key == Key.Insert)
                BackgroundImg.ToggleOpacity();
            else if (e.Key == Key.Enter)
                BackgroundImg.ChooseImg();
            else if (e.Key == Key.Space)
                BackgroundImg.Reset();
        }
    }
}
