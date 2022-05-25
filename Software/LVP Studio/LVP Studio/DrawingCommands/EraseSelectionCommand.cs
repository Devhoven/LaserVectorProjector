using ProjectorInterface.DrawingTools;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectorInterface.DrawingCommands
{
    // Removes the shape from the current canvas and adds it again, if necessary
    public class EraseSelectionCommand : CanvasCommand
    {
        Shape Shape;
        double width, height;
        HashSet<Shape> elements;

        public EraseSelectionCommand(Shape selection) : base(selection.StrRep() + "Erase.png")
        {
            Shape = selection;
            width = selection.Width;
            height = selection.Height;
            elements = ((SelectionRectangle)selection).selectedShapes;
        }

        public override void Execute()
        {
            Shape.Width = 0;
            Shape.Height = 0;

            foreach (Shape shape in elements)
            {
                Parent.Children.Remove(shape);
            }
        }

        public override void Undo()
        {
            Shape.Width = width;
            Shape.Height = height;

            foreach (Shape shape in elements)
            {
                shape.Stroke = Brushes.Red;
                Parent.Children.Add(shape);
            }
        }

        public override string ToString()
            => "Erase " + "Selection";

        public override BitmapFrame GetBmpFrame()
            => Icons[Shape.StrRep() + "Erase.png"];
    }
}
