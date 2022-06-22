using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectorInterface.GalvoInterface.UIElements
{
    class FramePanel : StackPanel
    {
        public static readonly DependencyProperty FrameDisplayContentProperty =
            DependencyProperty.Register("FrameDisplayContent", typeof(Label), typeof(FramePanel));

        public Label FrameDisplayContent
        {
            get { return (Label)GetValue(FrameDisplayContentProperty); }
            set { SetValue(FrameDisplayContentProperty, value); }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualRemoved != null)
                FrameDisplayContent.Content = "Frames: " + (Children.Count == 1 ? "" : (Children.Count - 1));
            else
                FrameDisplayContent.Content = "Frames: " + (Children.Count);
        }

    }
}
