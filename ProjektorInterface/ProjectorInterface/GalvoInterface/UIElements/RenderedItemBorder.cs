using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectorInterface.GalvoInterface.UIElements
{
    // A border which holds an image and has to be added into some kind of panel
    class RenderedItemBorder : Border
    {
        static readonly Thickness BORDER_THICKNESS = new Thickness(1);
        static readonly Thickness MARGIN = new Thickness(20, 10, 20, 10);
        static readonly Brush BORDER_BRUSH = Brushes.Black;

        static readonly Thickness SELECTED_BORDER_THICKNESS = new Thickness(3);
        static readonly Brush SELECTED_BORDER_BRUSH = Brushes.LightBlue;

        public RenderedItemBorder(Image child)
        {
            Child = child;
            Margin = MARGIN;
            Deselect();
        }

        public void RemoveFromParent()
            => ((Panel)Parent).Children.Remove(this);

        public void Select()
        {
            BorderBrush = SELECTED_BORDER_BRUSH;
            BorderThickness = SELECTED_BORDER_THICKNESS;
        }

        public void Deselect()
        {
            BorderBrush = BORDER_BRUSH;
            BorderThickness = BORDER_THICKNESS;
        }
    }
}
