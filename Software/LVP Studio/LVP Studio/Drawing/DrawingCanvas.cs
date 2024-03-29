﻿using LvpStudio.DrawingCommands;
using LvpStudio.DrawingTools;
using LvpStudio.Helper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LvpStudio
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
        public SelectionRectangle SelectRect { get; private set; }

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

            CanvasCommand.Parent = this;

            Commands = new CommandHistory();
            Children.Add(new CommandDisplay(Commands));

            SelectRect = new SelectionRectangle(Commands);
            SelectRect.Visibility = Visibility.Visible;
            Children.Add(SelectRect);

            // Got this from https://stackoverflow.com/a/9624985
            // Focuses the drawing canvas as soon as it gets visible
            IsVisibleChanged += (s, e) =>
            {
                if ((bool)e.NewValue)
                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, () => Focus());
            };
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
                if (SelectRect.IsSelecting)
                {
                    // Ctrl + Click Adds single Shape to selected Shapes
                    if (e.OriginalSource is Shape shape && Keyboard.IsKeyDown(Key.LeftCtrl))
                        SelectRect.SelectShape(shape);
                    else // Select multiple Shapes with SelectionRectangle
                    {
                        SelectRect.StartPos = e.GetPosition(this);
                        SelectRect.LastPos = SelectRect.StartPos;
                        SelectRect.DeselectAll();
                    }
                }
                // Drawing Shapes
                else
                {
                    SelectRect.DeselectAll();
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
                    Commands.Execute(new EraseSelectionCommand(SelectRect));
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
                if (e.LeftButton == MouseButtonState.Pressed && !SelectRect.IsSelecting)
                {
                    CurrentTool.Render(StartMousePos, e.GetPosition(this));
                }
                // Selecting with left Mouse Button
                else if (e.LeftButton == MouseButtonState.Pressed && SelectRect.IsSelecting)
                {
                    SelectRect.Render(SelectRect.StartPos, e.GetPosition(this));
                }
            }
        }

        // If the user releases one of the mouse buttons the tool visuals are going to be removed 
        // and potentially a new shape gets added to the canvas
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!LeftCanvas)
            {
                if (e.ChangedButton == MouseButton.Left && !SelectRect.IsSelecting)
                    RemoveToolAndCopy();
                else if (e.ChangedButton == MouseButton.Left && SelectRect.IsSelecting)
                {
                    if (SelectRect.StartPos == e.GetPosition(this))
                    {
                        SelectRect.Width = 0;
                        SelectRect.Height = 0;
                        SelectRect.DeselectAll();
                    }
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        SelectRect.ApplySelection();
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // If the mouse leaves the canvas, and the left mouse button was still pressed the operation is going to cancel
            // Doesn't work for some reason if you move onto the windows taskbar
            if (e.LeftButton == MouseButtonState.Pressed && !LeftCanvas && !SelectRect.IsSelecting){
                RemoveToolAndCopy();
                LeftCanvas = true;
            }
        }

        // Updates the CurrentTool and deselects all potentially selected shapes
        public void UpdateTool(DrawingTool tool)
        {
            CurrentTool = tool;
            SelectRect.IsSelecting = false;
            SelectRect.DeselectAll();
        }

        // Removes the visuals of the currently used tool from the canvas and adds the drawn shape onto it
        void RemoveToolAndCopy()
        {
            Children.Remove(CurrentTool);
            // Does not add a new shape, if the user did not move the mouse
            if (CurrentTool.HasChanged())
            {
                Commands.Execute(new AddShapeCommand(CurrentTool.CopyShape()));
                CurrentTool.Reset();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // The user can select a drawing mode with the keys 1 - 5
            if (Keybinds.IsPressed("Line"))
                UpdateTool(new LineTool());
            else if (Keybinds.IsPressed("Rectangle"))
                UpdateTool(new RectTool());
            else if (Keybinds.IsPressed("Ellipse"))
                UpdateTool(new CircleTool());
            else if (Keybinds.IsPressed("Path"))
                UpdateTool(new PathTool());
            else if (Keybinds.IsPressed("Selection"))
                SelectRect.IsSelecting = true;

            else if (e.Key == Key.Delete && SelectRect.IsSelecting && SelectRect.SelectedShapes.Count != 0)
                Commands.Execute(new EraseSelectionCommand(SelectRect));
            else if (e.Key == Key.Escape)
                SelectRect.DeselectAll();

            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // CTRL + Z = Undo
                if (Keyboard.IsKeyDown(Key.Z))
                    Commands.Undo();
                // CTRL + Y = Redo
                else if (Keyboard.IsKeyDown(Key.Y))
                    Commands.Redo();
                else if (Keyboard.IsKeyDown(Key.A))
                    SelectRect.SelectAll();
            }

            // Operations for the background image
            if (e.Key == Key.Insert)
                BackgroundImg.ToggleOpacity();
            else if (Keybinds.IsPressed("LoadBgImg"))
                BackgroundImg.ChooseImg();
            else if (e.Key == Key.Space)
                BackgroundImg.Reset();
            
            Keyboard.Focus(this);
        }

        public void ChooseImg()
            => BackgroundImg.ChooseImg();
    }
}
