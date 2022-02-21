using ProjectorInterface.DrawingCommands;
using ProjectorInterface.GalvoInterface.UIElements;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ProjectorInterface.GalvoInterface.UiElements
{
    partial class RenderedFrame : Grid
    {
        VectorizedFrame Frame;

        Button DeleteBtn = null!;
        Button SettingsBtn = null!;
        Panel SpeedBtnsPanel;

        public RenderedFrame(VectorizedFrame frame)
        {
            // Saving the frame, so I can delete it later from the image
            Frame = frame;

            // Rendering the frame onto an image control
            Image renderedFrame = new Image();
            renderedFrame.Source = frame.GetRenderedFrame();
            RenderOptions.SetBitmapScalingMode(renderedFrame, BitmapScalingMode.Fant);
            Children.Add(renderedFrame);

            CreateDeleteBtn();
            CreateSettingsBtn();

            SpeedBtnsPanel = new SpeedBtnPanel(Frame);
            SpeedBtnsPanel.MouseEnter += (o, e) 
                => ((Storyboard)Application.Current.Resources["FadeIn"]).Begin(SpeedBtnsPanel);

            CreateButtonAnimations();
                
            // Adding them to the frame
            Children.Add(DeleteBtn);
            Children.Add(SettingsBtn);
            Children.Add(SpeedBtnsPanel);
        }

        void CreateDeleteBtn()
        {
            // Creating the delete button
            DeleteBtn = new Button()
            {
                Content = new Image()
                {
                    Source = AssetManager.GetBmpFrame("Delete.png"),
                    Margin = new Thickness(3)
                },
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Opacity = 0,
                Style = (Style)Application.Current.FindResource("SmallButton")
            };

            DeleteBtn.Click += DeleteBtnClick;
        }

        void CreateSettingsBtn()
        {
            // Creating the settings button
            SettingsBtn = new Button()
            {
                Content = new Image()
                {
                    Source = AssetManager.GetBmpFrame("Settings.png"),
                    Margin = new Thickness(3)
                },
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Opacity = 0,
                Style = (Style)Application.Current.FindResource("SmallButton")
            };

            SettingsBtn.Click += (o, e) 
                => AnimationPlayer.FadeIn(SpeedBtnsPanel);
        }

        // Creating and adding the delete and settings button
        void CreateButtonAnimations()
        {
            // If the mouse enters the frame, the buttons fade in 
            MouseEnter += (o, e) =>
            {
                AnimationPlayer.FadeIn(DeleteBtn);
                AnimationPlayer.FadeIn(SettingsBtn);
            };

            // If the mouse leaves the frame, the buttons fade out
            MouseLeave += (o, e) =>
            {
                AnimationPlayer.FadeOut(DeleteBtn);
                AnimationPlayer.FadeOut(SettingsBtn);
                AnimationPlayer.FadeOut(SpeedBtnsPanel);
            };
        }

        private void DeleteBtnClick(object sender, RoutedEventArgs e)
            => DeleteFrame();

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Deleting itself from the VectorziedImage it belogned to and from the stackpanel
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                DeleteFrame();
                e.Handled = true;
            }
        }

        void DeleteFrame()
        {
            ShapesToPoints.DrawnImage.RemoveFrame(Frame);
            ((RenderedItemBorder)Parent).RemoveFromParent();
        }
    }

    class SpeedBtnPanel : WrapPanel
    {
        // Defines which buttons are going to be in the panel
        static readonly List<(string, ushort)> SPEED_BTNS = new List<(string, ushort)>()
        {
            ("1f", 1),
            ("12f", 12),
            ("24f",   24)
        };

        // Referencing it here, so I can alter how many times the image has to be replayed
        VectorizedFrame Frame;

        static readonly Brush SELECTED_BORDER_BRUSH = Brushes.LightBlue;
        static readonly Brush UNSELECTED_BORDER_BRUSH = Brushes.Black;

        public SpeedBtnPanel(VectorizedFrame frame)
        {
            Frame = frame;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0, 31, 0, 0);
            Visibility = Visibility.Hidden;
            Orientation = Orientation.Vertical;

            // Just loops through all of the entries in the speedBtns - list and creates buttons out of them
            for (int i = 0; i < SPEED_BTNS.Count; i++)
            {
                (string name, ushort value) = SPEED_BTNS[i];
                Button btn = new Button()
                {
                    Content = name,
                    BorderBrush = UNSELECTED_BORDER_BRUSH,
                    Style = (Style)Application.Current.Resources["SmallButton"]
                };
                
                // If the current button has the right amount of repeats, it is going to be highlighted
                if (value == frame.ReplayCount)
                    btn.BorderBrush = SELECTED_BORDER_BRUSH;

                // If one of the buttons get clicked, the appropriate number of repeats is going to be set
                // It further deselects all of the other buttons in this panel and highlights itself
                btn.Click += (sender, args) =>
                {
                    Frame.ReplayCount = value;
                    for (int i = 0; i < Children.Count; i++)
                        ((Button)Children[i]).BorderBrush = UNSELECTED_BORDER_BRUSH;
                    btn.BorderBrush = SELECTED_BORDER_BRUSH;
                };

                Children.Add(btn);
            }
        }
    }
}
