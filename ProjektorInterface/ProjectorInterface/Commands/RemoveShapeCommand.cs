using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ProjectorInterface.Commands
{
    // Removes the shape from the current canvas and readds it, if necessary
    class RemoveShapeCommand : DrawCommand
    {
        Shape Shape;

        public RemoveShapeCommand(Shape shape)
        {
            Shape = shape;
        }

        public override void Execute()
            => Parent.Children.Remove(Shape);

        public override void Undo()
            => Parent.Children.Add(Shape);
    }
}
