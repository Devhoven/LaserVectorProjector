using ProjectorInterface.Helper;
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
    // Removes the shape from the current canvas and adds it again, if necessary
    public class EraseShapeCommand : CanvasCommand
    {
        Shape Shape;

        public EraseShapeCommand(Shape shape)
            => Shape = shape;

        public override void Execute()
            => Parent.Children.Remove(Shape);

        public override void Undo()
            => Parent.Children.Add(Shape);

        public override string ToString()
            => "Erase " + Shape.StrRep();

        public override Stream GetImgFile()
        {
            return AssetManager.GetStream(Shape.StrRep() + "Erase.png");
        }
    }
}
