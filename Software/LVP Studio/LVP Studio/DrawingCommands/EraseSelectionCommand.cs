using LvpStudio.DrawingTools;
using LvpStudio.Helper;
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

namespace LvpStudio.DrawingCommands
{
    // Removes the shape from the current canvas and adds it again, if necessary
    public class EraseSelectionCommand : CanvasCommand
    {
        readonly SelectionRectangle SelectRect;
        readonly HashSet<Shape> SelectedElements;

        public EraseSelectionCommand(SelectionRectangle selectRect) : base(selectRect.StrRep() + "Erase.png")
        {
            SelectRect = selectRect;
            SelectedElements = new HashSet<Shape>(SelectRect.SelectedShapes);
        }

        public override void Execute()
        {
            SelectRect.Width = 0;
            SelectRect.Height = 0;

            foreach (Shape shape in SelectedElements)
            {
                Parent.Children.Remove(shape);
            }
        }

        public override void Undo()
        {
            foreach (Shape shape in SelectedElements)
            {
                shape.Stroke = Brushes.Red;
                Parent.Children.Add(shape);
            }
        }

        public override string ToString()
            => "Erase " + "Selection";

        public override BitmapFrame GetBmpFrame()
            => Icons[SelectRect.StrRep() + "Erase.png"];
    }
}
