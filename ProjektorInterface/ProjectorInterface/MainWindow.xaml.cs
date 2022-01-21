using Ookii.Dialogs.Wpf;
using ProjectorInterface.Commands;
using ProjectorInterface.GalvoInterface;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectorInterface
{
    public partial class MainWindow : Window
    {
        // Makes the value accessible in the XAML code
        public static readonly DependencyProperty CanvasResolutionProperty =
            DependencyProperty.Register("CanvasResolution", typeof(int), typeof(MainWindow),
                new PropertyMetadata(Settings.CANVAS_RESOLUTION));

        CommandHistoryWindow HistoryWindow = null!;

        public MainWindow()
        {
            InitializeComponent();

            SerialManager.Initialize("COM5");

            // You somehow can't override this event
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // The owner of this window is only be able to set if the main window is open already
            HistoryWindow = new CommandHistoryWindow(DrawCon.Commands);
            HistoryWindow.Owner = this;
            // This way it starts in the top right corner of the window
            HistoryWindow.Left = this.Left + this.Width - HistoryWindow.Width;
            HistoryWindow.Top = this.Top + 80;
            HistoryWindow.Show();

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                Combo.Items.Add(port);
            }
            Combo.SelectedIndex = 0;

            // Did this, so the canvas would get the focus of the keyboard
            Keyboard.Focus(DrawCon);
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
