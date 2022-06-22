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
    // Removes the shape from the current canvas and adds it again, if necessary
    public class EraseShapeCommand : CanvasCommand
    {
        readonly Shape Shape;

        public EraseShapeCommand(Shape shape) : base(shape.StrRep() + "Erase.png") 
            => Shape = shape;

        public override void Execute()
            => Parent.Children.Remove(Shape);

        public override void Undo()
            => Parent.Children.Add(Shape);

        public override string ToString()
            => "Erase " + Shape.StrRep();

        public override BitmapFrame GetBmpFrame()
            => Icons[Shape.StrRep() + "Erase.png"];
    }
}
