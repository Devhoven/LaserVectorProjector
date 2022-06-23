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
using System.Windows.Media.Imaging;

namespace ProjectorInterface
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance
        {
            get => _Instance;
        }

        // Contains a reference to the currently running MainWindow
        static MainWindow _Instance = null!;

        public MainWindow()
        {
            _Instance = this;

            InitializeComponent();
			
            // You somehow can't override this event
            Loaded += OnLoaded;

            DrawCon.CurrentToolChanged += CurrentTool_CurrentToolChanged;
            DrawCon.SelectRect.SelectionChanged += IsSelecting_SelectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Did this, so the canvas would get the focus of the keyboard
            Keyboard.Focus(DrawCon);

            AnimationManager.AddImage(AnimationManager.Source.UserImage, ShapesToPoints.DrawnImage);
        }

        // Stopping the thread if it still runs
        protected override void OnClosed(EventArgs e)
            => AnimationManager.StopCurrentThread();

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

        private void SkipAnimationClick(object sender, RoutedEventArgs e)
            => AnimationManager.SkipAnimation(AnimationManager.Source.AnimationGallery);

        private void RevertAnimationClick(object sender, RoutedEventArgs e)
            => AnimationManager.RevertAnimation(AnimationManager.Source.AnimationGallery);

        private void SelectLineClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new LineTool());
        
        private void SelectRectangleClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new RectTool());

        private void SelectCircleClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new CircleTool());
        
        private void SelectPathClick(object sender, RoutedEventArgs e)
            => DrawCon.UpdateTool(new PathTool());

        private void SelectionClick(object sender, RoutedEventArgs e)
            => DrawCon.SelectRect.IsSelecting = true;

        private void SaveCanvasClick(object sender, RoutedEventArgs e)
            => SaveCanvasDialog();

        private void LoadImageClick(object sender, RoutedEventArgs e)
            => DrawCon.ChooseImg();

        private static void SaveCanvasDialog()
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
                IldEncoder.EncodeImg(dialog.FileName, ShapesToPoints.DrawnImage);
        }

        private void SelectShowFolderDialog()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                // Loading the new images in 
                AnimationManager.LoadImagesFromFolder(dialog.SelectedPath);

                // Clearing all of the old animations and adding the new ones
                AnimationGallery.Clear();
                foreach (VectorizedImage img in AnimationManager.GetAnimation(AnimationManager.Source.AnimationGallery).Images)
                    AnimationGallery.AddBorder(new RenderedItemBorder(new RenderedImage(img)));
            }
        }

        private void AddDrawnFrame()
        {
            // Converts the shapes from the canvas into a frame and writes appends it to ShapesToPoints.DrawnImage
            ShapesToPoints.CalcFrameFromCanvas();
            FramePanel.Children.Add(new RenderedItemBorder(
                new RenderedFrame(
                    ShapesToPoints.DrawnImage.Frames[ShapesToPoints.DrawnImage.FrameCount - 1])));
        }
    }
}
