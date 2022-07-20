using LvpStudio.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LvpStudio.DrawingTools
{
    // Superclass for all of the drawing tools, like the line etc.
    public class DrawingTool : Border
    {
        protected Shape Current;
        private Point Start;
        private Point End;

        // Only for the subclasses
        protected DrawingTool(Shape current)
        {
            Current = current;
            Child = Current;

            Current.StrokeThickness = Settings.SHAPE_THICKNESS;
            Current.Stroke = Settings.SHAPE_COLOR;
            Current.Width = 0;
            Current.Height = 0;
        }

        // Hast to be called on every MouseMove - event, with the position of the mouse when the user clicked on the canvas 
        // and the current mouse pos
        // It updates the current tool
        public void Render(Point start, Point end)
        {
            Start = start;
            End = end;
            _Render(Start, End);
        }
        
        protected virtual void _Render(Point start, Point end)
        { }

        // Copy returns a copy of the shape which the tool is supposed to draw
        public virtual Shape CopyShape()
            => Current;

        public bool HasChanged()
            => Start != End;

        public void Reset()
            => Start = End;
    }
}
