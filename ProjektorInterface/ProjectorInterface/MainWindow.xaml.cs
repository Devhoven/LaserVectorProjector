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
        // Contains a reference to the currently running MainWindow
        public static MainWindow Instance = null!;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();
			
            // You somehow can't override this event
            Loaded += OnLoaded;

            DrawCon.CurrentToolChanged += CurrentTool_CurrentToolChanged;
            DrawCon.Selection.SelectionChanged += IsSelecting_SelectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Did this, so the canvas would get the focus of the keyboard
            Keyboard.Focus(DrawCon); 
        }

        // Stopping the thread if it still runs
        protected override void OnClosed(EventArgs e)
            => AnimationManager.Stop();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // CTRL + SHIFT + S = Save your drawing as an .ild file 
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    SaveCanvasDialog();
                else
                    AddDrawnFrame();
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
                SelectShowFolderDialog();
            else if (e.Key == Key.C)
                new PortSelectWindow(this).ShowDialog();

            Keyboard.Focus(DrawCon);
        }

        // Handler for changed drawingTool currentTool
        private void CurrentTool_CurrentToolChanged(object? sender, EventArgs e)
            => BtnPanel.ToolChanged(DrawCon);
        private void IsSelecting_SelectionChanged(object? sender, EventArgs e)
            => BtnPanel.SelectButton((Button)BtnPanel.Children[4]);

        // Opens a folder dialog for selecting a folder with .ild files
        private void SelectShowClick(object sender, RoutedEventArgs e)
            => SelectShowFolderDialog();

        // Starts the show
        private void StartShowClick(object sender, RoutedEventArgs e)
            => AnimationManager.Start();

        // Stops it
        private void StopShowClick(object sender, RoutedEventArgs e)
            => AnimationManager.Stop();

        private void SelectLineClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new LineTool());
        
        private void SelectRectangleClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new RectTool());

        private void SelectCircleClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new CircleTool());
        
        private void SelectPathClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new PathTool());

        private void SelectionClick(object sender, RoutedEventArgs e)
            => DrawCon.Selection.isSelecting = true;

        private void ProjectCanvasClick(object sender, RoutedEventArgs e)
            => ProjectCanvas();

        private void SaveCanvasClick(object sender, RoutedEventArgs e)
            => SaveCanvasDialog();

        private void LoadImageClick(object sender, RoutedEventArgs e)
            => DrawCon.BackgroundImg.ChooseImg();

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

        private void SelectShowFolderDialog()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                // Clearing the old images from the queue
                AnimationManager.ClearImages();
                // Loading the new ones in
                AnimationManager.LoadImagesFromFolder(dialog.SelectedPath);

                // Clearing all of the old animations and adding the new ones
                AnimationGallery.Clear();
                foreach (VectorizedImage img in AnimationManager.Images)
                    AnimationGallery.AddBorder(new RenderedItemBorder(new RenderedImage(img)));
            }
        }

        private void ProjectCanvas()
        {
            if (ShapesToPoints.DrawnImage.FrameCount > 0)
            {
                AnimationManager.ClearImages();
                AnimationManager.AddImage(ShapesToPoints.DrawnImage);
                AnimationManager.Start();
            }
        }

        private void AddDrawnFrame()
        {
            // Converts the shapes from the canvas into a frame and writes appends it to ShapesToPoints.DrawImage
            ShapesToPoints.CalcFrameFromCanvas();
            FramePanel.Children.Add(new RenderedItemBorder(
                new RenderedFrame(
                    ShapesToPoints.DrawnImage.Frames[ShapesToPoints.DrawnImage.FrameCount - 1])));
        }
    }
}
