using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static ProjectorInterface.Helper.Settings;

namespace ProjectorInterface
{
    // A image which can be moved and zoomed
    public class MoveableImage : Image
    {
        double _Top, _Left;
        bool IsDragging = false;
        Point StartPos;
        Point LastPos;

        private double Y
        {
            get => _Top;
            set
            {
                _Top = value;
                Canvas.SetTop(this, _Top);
            }
        }

        double X
        {
            get => _Left;
            set
            {
                _Left = value;
                Canvas.SetLeft(this, _Left);
            }
        }

        public MoveableImage()
        {
            Opacity = 255;
            RenderTransform = new ScaleTransform(1, 1);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            IsDragging = true;
            StartPos = e.GetPosition((Canvas)Parent);
            LastPos = StartPos;
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && IsDragging)
            {
                Point currentPos = e.GetPosition((Canvas)Parent);
                Vector diff = Point.Subtract(LastPos, currentPos);
                LastPos = currentPos;

                X -= diff.X;
                Y -= diff.Y;

                e.Handled = true;
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                IsDragging = false;
                Point currentPos = e.GetPosition((Canvas)Parent);
                if (Point.Subtract(currentPos, StartPos).LengthSquared > 5)
                    e.Handled = true;
            }
        }

        // Modified from https://stackoverflow.com/a/6782715/9241163
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var st = (ScaleTransform)RenderTransform;

            double zoom = e.Delta > 0 ? .1 : -.1;
            if (e.Delta <= 0 && (st.ScaleX < .4 || st.ScaleY < .4))
                return;

            Point relative = e.GetPosition(this);
            double absoluteX;
            double absoluteY;

            absoluteX = relative.X * st.ScaleX + X;
            absoluteY = relative.Y * st.ScaleY + Y;

            double zoomCorrected = zoom * st.ScaleX; 
            st.ScaleX += zoomCorrected; 
            st.ScaleY += zoomCorrected;

            X = absoluteX - relative.X * st.ScaleX;
            Y = absoluteY - relative.Y * st.ScaleY;
        }

        // Disables and shows the image again
        public void ToggleOpacity()
            => Opacity = Opacity != 0 ? 0 : 255;

        public void ChooseImg()
        {
            // Create OpenFileDialog 
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".png",
                Multiselect = false,
                Filter = "Image files (*.png; *.jpg; *.jpeg; *.bmp;)|*.png;*.jpg;*.jpeg;*.bmp;"
            };

            // Checking if the user selected something
            if (fileDialog.ShowDialog() == true)
            {
                Source = new BitmapImage(new Uri(fileDialog.FileName));
                Reset();
            }
        }

        // Resets the image to the width and height of the canvas and to the top left position
        public void Reset()
        {
            X = 0;
            Y = 0;
            StartPos.X = 0;
            StartPos.Y = 0;
            RenderTransform = new ScaleTransform(1, 1);
        }
    }
}
