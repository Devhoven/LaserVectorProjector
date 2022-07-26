using Ookii.Dialogs.Wpf;
using LvpStudio.DrawingTools;
using LvpStudio.GalvoInterface;
using LvpStudio.GalvoInterface.UiElements;
using LvpStudio.GalvoInterface.UIElements;
using LvpStudio.Helper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LvpStudio
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
            if (Keybinds.IsPressed("AddFrame"))
                AddDrawnFrame();
            else if (Keybinds.IsPressed("SaveAnimation"))
                SaveAnimationDialog();
            else if (Keybinds.IsPressed("ProjectFrame"))
                ProjectUserAnimBtn.ToggleAnimation(this, e);
            else if (Keybinds.IsPressed("LoadAnimations"))
                SelectShowFolderDialog();
            else if (Keybinds.IsPressed("LoadBgImg"))
                DrawCon.ChooseImg();
            else if (Keybinds.IsPressed("PlayPause"))
                ToggleAnimBtn.ToggleAnimation(this, e);
            else if (Keybinds.IsPressed("SkipAnimation"))
                SkipAnimationClick(this, e);
            else if (Keybinds.IsPressed("RevertAnimation"))
                RevertAnimationClick(this, e);
            else if (Keybinds.IsPressed("ShowPortSelectWindow"))
                OpenEditPortWindow();

            Keyboard.Focus(DrawCon);
        }

        #region Clickhandler

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

        private void SaveAnimationClick(object sender, RoutedEventArgs e)
            => SaveAnimationDialog();

        private void LoadImageClick(object sender, RoutedEventArgs e)
            => DrawCon.ChooseImg();

        private void OpenKeybindsClick(object sender, RoutedEventArgs e)
            => OpenEditKeybindsWindow();

        private void OpenPortClick(object sender, RoutedEventArgs e)
            => OpenEditPortWindow();

        private void ToggleDebugMode(object sender, RoutedEventArgs e)
            => Settings.DEBUG_MODE = !Settings.DEBUG_MODE;

        #endregion

        private static void SaveAnimationDialog()
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
            if (!Settings.DEBUG_MODE)
            {
                FramePanel.Children.Add(new RenderedItemBorder(
                    new RenderedFrame(
                        new VectorizedFrame(ShapesToPoints.Points.ToArray()))));
            }
            else
            {
                FramePanel.Children.Add(new RenderedItemBorder(
                    new RenderedFrame(ShapesToPoints.DrawnImage.Frames[^1])));
            }
        }

        private void OpenEditKeybindsWindow()
            => new HotkeyWindow(this).ShowDialog();
        private void OpenEditPortWindow()
            => new PortSelectWindow(this).ShowDialog();
    }
}
