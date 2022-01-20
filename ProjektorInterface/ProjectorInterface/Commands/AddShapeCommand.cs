using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ProjectorInterface.Commands
{
    // Saves the shape which was added to the canvas and deletes it if necessary
    class AddShapeCommand : DrawCommand
    {
        Shape Shape;
        public AddShapeCommand(Shape shape)
        {
            Shape = shape;
        }

        public override void Execute()
            => Parent.Children.Add(Shape);

        public override void Undo()
            => Parent.Children.Remove(Shape);
    }
}
