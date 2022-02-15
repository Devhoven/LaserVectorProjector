using ProjectorInterface.Commands;
using ProjectorInterface.GalvoInterface.UIElements;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectorInterface.GalvoInterface.UiElements
{
    class RenderedFrame : Image
    {
        VectorizedFrame Frame;

        public RenderedFrame(VectorizedFrame frame)
        {
            Frame = frame;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
            Source = frame.GetRenderedFrame();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Deleting itself from the VectorziedImage it belogned to and from the stackpanel
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ShapesToPoints.DrawnImage.RemoveFrame(Frame);
                ((RenderedItemBorder)Parent).RemoveFromParent();

                e.Handled = true;
            }
        }
    }
}
