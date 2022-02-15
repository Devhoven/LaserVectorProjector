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
    class RenderedItemBorder : Border
    {
        static readonly Thickness BORDER_THICKNESS = new Thickness(1);
        static readonly Thickness MARGIN = new Thickness(20, 10, 20, 10);

        public RenderedItemBorder(Image child)
        {
            Child = child;
            BorderBrush = Brushes.Black;
            BorderThickness = BORDER_THICKNESS;
            Margin = MARGIN;
        }

        public void RemoveFromParent()
            => ((Panel)Parent).Children.Remove(this);
    }
}
