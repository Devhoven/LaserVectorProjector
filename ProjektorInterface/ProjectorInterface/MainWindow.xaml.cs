using Ookii.Dialogs.Wpf;
using ProjectorInterface.Commands;
using ProjectorInterface.DrawingTools;
using ProjectorInterface.GalvoInterface;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectorInterface
{
    public partial class MainWindow : Window
    {
        // Makes the value accessible in the XAML code
        public static readonly DependencyProperty CommandHistoryProperty =
            DependencyProperty.Register("CommandHistory", typeof(CommandHistory), typeof(MainWindow),
                new PropertyMetadata(new CommandHistory()));

        public static readonly DependencyProperty RenderedImgSizeProperty =
            DependencyProperty.Register("RenderedImgSize", typeof(int), typeof(MainWindow),
                new PropertyMetadata(Settings.RENDERED_IMG_SIZE));

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.WhiteSmoke));

        // Contains a reference to the currently running MainWindow
        public static MainWindow Instance = null!;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            // You somehow can't override this event
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                SelectPortComboBox.Items.Add(port);
            }
            SelectPortComboBox.SelectedIndex = 0;

            SerialManager.Initialize("COM10");

            // Did this, so the canvas would get the focus of the keyboard
            Keyboard.Focus(DrawCon);
        }

        // Stopping the thread if it still runs
        protected override void OnClosed(EventArgs e)
            => SerialManager.Stop();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.D0) // 0-Key converts shapes into array of points which can be given to the Arduino
            {  
                //LineSegment[] points = ShapesToPoints.getPoints();
            }
            else if(e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Border imgBorder = new Border()
                {
                    Child = new RenderedImage(ShapesToPoints.getPoints(), FramePanel.Children.Count),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(20, 10, 20, 10)
                };
                FramePanel.Children.Add(imgBorder);
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SelectShowClick(null!, null!);
            }

            Keyboard.Focus(DrawCon);
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
            foreach (VectorizedImage img in SerialManager.Images)
            {
                Border imgBorder = new Border()
                {
                    Child = new RenderedImage(img),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(20, 10, 20, 10)
                };
                AnimationFramesGallery.Children.Add(imgBorder);
            }
        }

        // Starts the sending of data
        private void StartShowClick(object sender, RoutedEventArgs e)
            => SerialManager.Start();

        // Stops it
        private void StopShowClick(object sender, RoutedEventArgs e)
            => SerialManager.Stop();

        private void SelectPortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectPortComboBox.Text != "" && SelectPortComboBox.Items != null)
            {
                SerialManager.Initialize(SelectPortComboBox.Text);
            }
        }

        private void GitHubClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { 
                FileName = "https://github.com/Devhoven/LaserVectorProjector", 
                UseShellExecute = true 
            });
        }

        private void SelectLineClick(object sender, RoutedEventArgs e)
            => DrawCon.CurrentTool = new LineTool();

        private void SelectRectangleClick(object sender, RoutedEventArgs e)
            => DrawCon.CurrentTool = new RectTool();

        private void SelectCircleClick(object sender, RoutedEventArgs e)
            => DrawCon.CurrentTool = new CircleTool();
        
        private void SelectPathClick(object sender, RoutedEventArgs e)
            => DrawCon.CurrentTool = new PathTool();        

        private void MenuItem_Click(object sender, RoutedEventArgs e)
            => DrawCon.Children.Clear();
    }
}
