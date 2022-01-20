﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ProjectorInterface.DrawingTools
{
    // Allows the user to draw a circle
    class CircleTool : DrawingTool
    {
        Ellipse CirlceObj => (Ellipse)Current;

        Point CirclePos;

        public CircleTool() : base(new Ellipse())
        {
            CirclePos = new Point();
        }

        public override Shape CopyShape()
        {
            Ellipse tmp = new Ellipse()
            {
                StrokeThickness = Settings.SHAPE_THICKNESS,
                Stroke = Settings.SHAPE_COLOR,
                Width = CirlceObj.Width,
                Height = CirlceObj.Height
            };
            Canvas.SetLeft(tmp, CirclePos.X);
            Canvas.SetTop(tmp, CirclePos.Y);
            return tmp;
        }

        public override void Render(Point start, Point end)
        {
            Point topLeft = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point bottomRight = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double size = Math.Max(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                CirlceObj.Width = size;
                CirlceObj.Height = size;
            }
            else
            {
                CirlceObj.Width = bottomRight.X - topLeft.X;
                CirlceObj.Height = bottomRight.Y - topLeft.Y;
            }

            Canvas.SetLeft(this, CirclePos.X = topLeft.X);
            Canvas.SetTop(this, CirclePos.Y = topLeft.Y);
        }
    }
}
