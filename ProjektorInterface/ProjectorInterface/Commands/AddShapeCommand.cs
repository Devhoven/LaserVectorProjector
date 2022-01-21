﻿using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ProjectorInterface.Commands
{
    // Saves the shape which was added to the canvas and deletes it if necessary
    public class AddShapeCommand : CanvasCommand
    {
        Shape Shape;

        public AddShapeCommand(Shape shape)
            => Shape = shape;

        public override void Execute()
            => Parent.Children.Add(Shape);

        public override void Undo()
            => Parent.Children.Remove(Shape);

        public override string ToString()
            => "Draw " + Shape.StrRep();

        public override Stream GetImgFile()
        {
            return AssetManager.GetStream(Shape.StrRep() + ".png");
        }
    }
}