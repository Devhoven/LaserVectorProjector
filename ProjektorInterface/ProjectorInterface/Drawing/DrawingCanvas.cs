using ProjectorInterface.Commands;
using ProjectorInterface.DrawingTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ProjectorInterface
{
    // A canvas the user is able to draw on
    public class DrawingCanvas : Canvas
    {
        // Gets set when the user clicks on the canvas
        Point StartMousePos;

        // Holds all of the actions the user did, like drawing or deleting something
        CommandHistory Commands;

        // Holds the current tool one is drawing with
        DrawingTool CurrentTool;

        // Background image of the canvas
        // Can be controlled, so the user is able to trace the outlines
        MoveableImage BackgroundImg;

        public DrawingCanvas()
        {
            CurrentTool = new LineTool();
            Commands = new CommandHistory();

            BackgroundImg = new MoveableImage();
            Children.Add(BackgroundImg);

            DrawCommand.Parent = this;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Getting the mouse pos relative to the canvas
            StartMousePos = e.GetPosition(this);
            // Resetting the position of the current tool, which makes it invisible at the start
            // Removes a bug
            CurrentTool.Render(StartMousePos, StartMousePos);

            // If the user middleclicked on one of the shapes, it is going to be deleted
            if (e.MiddleButton == MouseButtonState.Pressed && e.OriginalSource is Shape)
            {
                Commands.Execute(new RemoveShapeCommand((Shape)e.OriginalSource));
                return;
            }

            Children.Add(CurrentTool);
        }

        // As long as the mouse moves and the left mouse button is pressed, the currently selected tool updates its visuals
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                CurrentTool.Render(StartMousePos, e.GetPosition(this));
        }

        // If the user releases one of the mouse buttons the tool visuals are going to be removed 
        // and potentially a new shape gets added to the canvas
        protected override void OnMouseUp(MouseButtonEventArgs e)
            => RemoveToolAndCopy(e.GetPosition(this));

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
                // CTRL + Z = Redo
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

            // For moving the background image 
            if (Keyboard.IsKeyDown(Key.Up))
                BackgroundImg.MoveUp();
            else if (Keyboard.IsKeyDown(Key.Down))
                BackgroundImg.MoveDown();

            if (Keyboard.IsKeyDown(Key.Left))
                BackgroundImg.MoveLeft();
            else if (Keyboard.IsKeyDown(Key.Right))
                BackgroundImg.MoveRight();
        }
    }
}
