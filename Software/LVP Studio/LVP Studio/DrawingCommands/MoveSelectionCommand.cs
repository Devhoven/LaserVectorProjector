using LvpStudio.DrawingCommands;
using LvpStudio.DrawingTools;
using LvpStudio.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LVP_Studio.DrawingCommands
{
    public class MoveSelectionCommand : CanvasCommand
    {
        readonly SelectionRectangle SelectRect;
        readonly double RectLeft, RectTop;
        readonly double RectWidth, RectHeight;
        readonly Vector MoveDiff;
        readonly HashSet<Shape> SelectedElements;

        public MoveSelectionCommand(SelectionRectangle selectRect, double rectLeft, double rectTop, double rectWidth, double rectHeight, Vector moveDiff) : base(selectRect.StrRep() + "Move.png")
        {
            SelectRect = selectRect;

            RectLeft = rectLeft;
            RectTop = rectTop;
            RectWidth = rectWidth;
            RectHeight = rectHeight;

            MoveDiff = moveDiff;

            SelectRect = selectRect;
            SelectedElements = new HashSet<Shape>(SelectRect.SelectedShapes);
        }

        public override void Execute()
        {
            SelectRect.Left = RectLeft;
            SelectRect.Top = RectTop;
            SelectRect.Width = RectWidth;
            SelectRect.Height = RectHeight;

            MoveShapes(MoveDiff);

            SelectRect.SelectedShapes = new HashSet<Shape>(SelectedElements);
        }

        public override void Undo()
        {
            SelectRect.Left = RectLeft + MoveDiff.X;
            SelectRect.Top = RectTop + MoveDiff.Y;
            SelectRect.Width = RectWidth;
            SelectRect.Height = RectHeight;

            MoveShapes(-MoveDiff);

            SelectRect.SelectedShapes = new HashSet<Shape>(SelectedElements);
        }

        void MoveShapes(Vector moveDiff)
        {
            // Apply Movement for each selected Shape
            foreach (Shape s in SelectedElements)
            {
                Canvas.SetLeft(s, Canvas.GetLeft(s) - moveDiff.X);
                Canvas.SetTop(s, Canvas.GetTop(s) - moveDiff.Y);
                s.Stroke = Brushes.Blue;
            }
        }

        public override string ToString()
            => "Move Selection";

        public override BitmapFrame GetBmpFrame()
            => Icons[SelectRect.StrRep() + "Move.png"];
    }
}
