using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectorInterface.DrawingCommands
{
    // Saves the shape which was added to the canvas and deletes it if necessary
    public class AddShapeCommand : CanvasCommand
    {
        Shape Shape;

        public AddShapeCommand(Shape shape) : base(shape.StrRep() + ".png")
            => Shape = shape;

        public override void Execute()
            => Parent.Children.Add(Shape);

        public override void Undo()
            => Parent.Children.Remove(Shape);

        public override string ToString()
            => "Draw " + Shape.StrRep();

        public override BitmapFrame GetBmpFrame()
            => Icons[Shape.StrRep() + ".png"];
    }
}
