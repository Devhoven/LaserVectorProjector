using ProjectorInterface.DrawingCommands;
using ProjectorInterface.DrawingTools;
using ProjectorInterface.Helper;
using System;
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
        public event EventHandler? CurrentToolChanged;

        // Gets set when the user clicks on the canvas
        Point StartMousePos;

        // If the mouse left the canvas
        bool LeftCanvas = false;

        // Holds all of the actions the user did, like drawing or deleting something
        readonly CommandHistory Commands;

        // Selection Rectangle to select multiple shapes
        public SelectionRectangle Selection { get; private set; }

        // Holds the current tool one is drawing with
        DrawingTool _CurrentTool;
        public DrawingTool CurrentTool
        {
            get => _CurrentTool;
            set
            {
                _CurrentTool = value;
                OnCurrentToolChanged();
            }
        }

        // Background image of the canvas
        // Can be controlled, so the user is able to trace the outlines
        MoveableImage BackgroundImg;

        public DrawingCanvas()
        {
            _CurrentTool = new LineTool();

            BackgroundImg = new MoveableImage();
            Children.Add(BackgroundImg);

            Background = Brushes.White;
            ClipToBounds = true;
            Focusable = true;

            CanvasCommand.Parent = this;

            Commands = new CommandHistory();
            Children.Add(new CommandDisplay(Commands));

            Selection = new SelectionRectangle();
            Selection.Visibility = Visibility.Visible;
            Children.Add(Selection);
        }

        private void OnCurrentToolChanged()
        {
            if (CurrentToolChanged != null)
                CurrentToolChanged(this, EventArgs.Empty);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Left click, either drawing or selecting
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                LeftCanvas = false;
                // Selecting Shapes
                if (Selection.IsSelecting)
                {
                    // Ctrl + Click Adds single Shape to selected Shapes
                    if (e.OriginalSource is Shape shape && Keyboard.IsKeyDown(Key.LeftCtrl))
                        Selection.SelectShape(shape);
                    else // Select multiple Shapes with SelectionRectangle
                    {
                        Selection.StartPos = e.GetPosition(this);
                        Selection.LastPos = Selection.StartPos;
                        Selection.DeselectAll();
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
            if (!LeftCanvas)
            {
                // Drawing with left Mouse Button
                if (e.LeftButton == MouseButtonState.Pressed && !Selection.IsSelecting)
                {
                    CurrentTool.Render(StartMousePos, e.GetPosition(this));
                }
                // Selecting with left Mouse Button
                else if (e.LeftButton == MouseButtonState.Pressed && Selection.IsSelecting)
                {
                    Selection.Render(Selection.StartPos, e.GetPosition(this));
                }
            }
        }

        // If the user releases one of the mouse buttons the tool visuals are going to be removed 
        // and potentially a new shape gets added to the canvas
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!LeftCanvas)
            {
                if (e.ChangedButton == MouseButton.Left && !Selection.IsSelecting)
                    RemoveToolAndCopy(e.GetPosition(this));
                else if (e.ChangedButton == MouseButton.Left && Selection.IsSelecting)
                {
                    if (Selection.StartPos == e.GetPosition(this))
                    {
                        Selection.Width = 0;
                        Selection.Height = 0;
                        Selection.DeselectAll();
                    }
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        Selection.ApplySelection();
                }
                Keyboard.Focus(this);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // If the mouse leaves the canvas, and the left mouse button was still pressed the operation is going to cancel
            // Doesn't work for some reason if you move onto the windows taskbar
            if (e.LeftButton == MouseButtonState.Pressed && !LeftCanvas){
                RemoveToolAndCopy(e.GetPosition(this));
                LeftCanvas = true;
            }
        }

        // Updates the CurrentTool and deselects all potentially selected shapes
        public void UpdateTool(DrawingTool tool)
        {
            CurrentTool = tool;
            Selection.IsSelecting = false;
            Selection.DeselectAll();
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
            // The user can select a drawing mode with the keys 1 - 5
            if (e.Key == Key.D1)
                UpdateTool(new LineTool());
            else if (e.Key == Key.D2)
                UpdateTool(new RectTool());
            else if (e.Key == Key.D3)
                UpdateTool(new CircleTool());
            else if (e.Key == Key.D4)
                UpdateTool(new PathTool());
            else if (e.Key == Key.D5)
                Selection.IsSelecting = true;

            else if (e.Key == Key.Delete && Selection.IsSelecting && Selection.SelectedShapes.Count != 0)
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

        public void ChooseImg()
            => BackgroundImg.ChooseImg();
    }
}
