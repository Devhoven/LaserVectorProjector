using Ookii.Dialogs.Wpf;
using ProjectorInterface.DrawingCommands;
using ProjectorInterface.DrawingTools;
using ProjectorInterface.GalvoInterface;
using ProjectorInterface.GalvoInterface.UiElements;
using ProjectorInterface.GalvoInterface.UIElements;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            SerialManager.Initialize("COM13");

            // Did this, so the canvas would get the focus of the keyboard
            Keyboard.Focus(DrawCon);
        }

        // Stopping the thread if it still runs
        protected override void OnClosed(EventArgs e)
            => SerialManager.Stop();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // CTRL + SHIFT + S = Save your drawing as an .ild file 
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    SaveCanvasDialog();
                }
                else
                {
                    // Converts the shapes from the canvas into a frame and writes appends it to ShapesToPoints.DrawImage
                    ShapesToPoints.CalcFrameFromCanvas();
                    FramePanel.Children.Add(new RenderedItemBorder(
                        new RenderedFrame(
                            ShapesToPoints.DrawnImage.Frames[ShapesToPoints.DrawnImage.FrameCount - 1])));
                }
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
                SelectShowClick(null!, null!);
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

                // Clearing all of the old animations and adding the new ones
                AnimationGallery.Clear();
                foreach (VectorizedImage img in SerialManager.Images)
                    AnimationGallery.AddBorder(new RenderedItemBorder(new RenderedImage(img)));
            }
        }

        // Starts the show
        private void StartShowClick(object sender, RoutedEventArgs e)
            => SerialManager.Start();

        // Stops it
        private void StopShowClick(object sender, RoutedEventArgs e)
            => SerialManager.Stop();

        private void SelectPortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectPortComboBox.Text != "" && SelectPortComboBox.Items != null)
                SerialManager.Initialize(SelectPortComboBox.Text);
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
        
        private void ClearCanvasClick(object sender, RoutedEventArgs e) 
            => DrawCon.Children.RemoveRange(1, DrawCon.Children.Count - 1);

        private void SelectionClick(object sender, RoutedEventArgs e)
            => DrawCon.CurrentTool = new SelectionTool();

        private void ProjectCanvasClick(object sender, RoutedEventArgs e)
        {
            if (ShapesToPoints.DrawnImage.FrameCount > 0)
            {
                SerialManager.ClearImages();
                SerialManager.AddImage(ShapesToPoints.DrawnImage);
                SerialManager.Start();
            }
        }

        private void SaveCanvasClick(object sender, RoutedEventArgs e)
            => SaveCanvasDialog();

        private void SaveCanvasDialog()
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog()
            {
                Title = "Save your drawing",
                DefaultExt = ".ild",
                AddExtension = true,
                CheckPathExists = true,
                Filter = "ILDA File | *.ild"
            };
            if (dialog.ShowDialog() == true)
                ILDEncoder.EncodeImg(dialog.FileName, ShapesToPoints.DrawnImage);
        }
    }
}
