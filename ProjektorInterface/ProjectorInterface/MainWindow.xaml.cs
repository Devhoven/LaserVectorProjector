using ProjectorInterface.Commands;
using ProjectorInterface.GalvoInterface;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ookii.Dialogs.Wpf;
using System;

namespace ProjectorInterface
{
    public partial class MainWindow : Window
    {
        // Makes the value accessible in the XAML code
        public static readonly DependencyProperty CanvasResolutionProperty =
            DependencyProperty.Register("CanvasResolution", typeof(int), typeof(MainWindow),
                new PropertyMetadata(Settings.CANVAS_RESOLUTION));

        public MainWindow()
        {
            InitializeComponent();

            SerialManager.Initialize("COM5");

            // Did this, so the canvas would get the focus of the keyboard
            Loaded += (sender, e) => Keyboard.Focus(DrawCon);
        }

        // Stopping the thread if it still runs
        protected override void OnClosed(EventArgs e)
            => SerialManager.Stop();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // ESC closes the app
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
            else if (e.Key == Key.D0)
            {
                List<LineSegment> tmp = ShapesToPoints.getPoints();
            }
        }

        // Opens a folder dialog for selecting a folder with .ild files
        private void SelectShowClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                // Clearing the old images from the queue
                SerialManager.ClearImages();
                // Loading the new ones in
                SerialManager.LoadImagesFromFolder(dialog.SelectedPath);
            }
        }

        // Starts the sending of data
        private void StartShowClick(object sender, RoutedEventArgs e)
            => SerialManager.Start();

        // Stops it
        private void StopShowClick(object sender, RoutedEventArgs e)
            => SerialManager.Stop();
    }
}
