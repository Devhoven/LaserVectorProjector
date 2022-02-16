using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        double Top
        {
            get => _Top;
            set => Canvas.SetTop(this, _Top = value);
        }

        double Left
        {
            get => _Left;
            set => Canvas.SetLeft(this, _Left = value);
        }

        public MoveableImage()
            => Opacity = 180;

        // The width and height gets altered for the illusion of zooming in and out 
        public void ZoomIn()
        {
            Width += ZOOM_IMG_SPEED;
            Height += ZOOM_IMG_SPEED;
        }
        public void ZoomOut()
        {
            Width = Math.Max(0, Width - ZOOM_IMG_SPEED);
            Height = Math.Max(0, Height - ZOOM_IMG_SPEED);
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

                Left -= diff.X;
                Top -= diff.Y;

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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double zoom = e.Delta > 0 ? 20 : -20;

            // Some arbitrary values to limit the how small / big the image can get
            if (Width + zoom < 300 || Width + zoom > ((FrameworkElement)Parent).ActualWidth * 2)
                return;

            Width += zoom;
            Height += zoom;
        }

        // Disables and shows the image again
        public void ToggleOpacity()
            => Opacity = Opacity == 255 ? 0 : 255;

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
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);

            Width = ((FrameworkElement)Parent).ActualWidth;
            Height = ((FrameworkElement)Parent).ActualHeight;
        }
    }
}
