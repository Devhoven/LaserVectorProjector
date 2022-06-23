using ProjectorInterface.GalvoInterface.UiElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.Forms.MessageBox;

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

        List<RenderedItemBorder> SelectedFrames = new List<RenderedItemBorder>();

        RenderedItemBorder? LastSelected = null;


        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualRemoved != null)
                FrameDisplayContent.Content = "Frames: " + (Children.Count == 1 ? "" : (Children.Count - 1));
            else
            {
                FrameDisplayContent.Content = "Frames: " + (Children.Count);
                ((RenderedItemBorder)visualAdded).MouseDown += Child_MouseDown;
            }
        }

        private void Child_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not RenderedItemBorder)
                return;

            RenderedItemBorder rib = (RenderedItemBorder)sender;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    int lastSelectedIndex = Children.IndexOf(LastSelected);
                    int currentIndex = Children.IndexOf(rib);
                    if (lastSelectedIndex > currentIndex)
                    {
                        (currentIndex, lastSelectedIndex) = (lastSelectedIndex, currentIndex);
                    }
                    for (int i = lastSelectedIndex; i <= currentIndex; i++)
                    {
                        ((RenderedItemBorder)Children[i]).Select();
                        SelectedFrames.Add((RenderedItemBorder)Children[i]);
                    }
                    return;
                }

                if (!rib.IsSelected)
                {
                    SelectedFrames.Add(rib);
                    rib.Select();
                    LastSelected = rib;
                }
                else
                {
                    SelectedFrames.Remove(rib);
                    rib.Deselect();
                }


            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (!ConfirmDeletion())
                    return;

                if (!rib.IsSelected)
                    Children.Remove(rib);


                foreach (RenderedItemBorder frame in SelectedFrames)
                {
                    Children.Remove(frame);
                }
                SelectedFrames.Clear();
            }
        }
        public bool ConfirmDeletion()
        {
            if (SelectedFrames.Count > 1)
            {
                DialogResult result = MessageBox.Show("Do you want to delete all currently selected frames?",
                                           "Delete selection",
                                           MessageBoxButtons.YesNo);

                return result == DialogResult.Yes;
            }
            else
            {
                return true;
            }
        }
    }
}
