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
        double Top = 0;
        double Left = 0;

        public MoveableImage()
        {
            Opacity = 255;
        }

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

        // Methods for moving the image through the canvas
        public void MoveUp()
            => Canvas.SetTop(this, Top = Top - MOVE_IMG_SPEED);
        public void MoveDown()
            => Canvas.SetTop(this, Top = Top + MOVE_IMG_SPEED);
        public void MoveLeft()
            => Canvas.SetLeft(this, Left = Left - MOVE_IMG_SPEED);
        public void MoveRight()
            => Canvas.SetLeft(this, Left = Left + MOVE_IMG_SPEED);

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
