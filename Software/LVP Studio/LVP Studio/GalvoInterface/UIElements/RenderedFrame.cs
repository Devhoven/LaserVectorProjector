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
        readonly VectorizedFrame Frame;

        readonly SpeedBtnPanel SpeedBtnPanel;
        readonly Button DeleteBtn;
        readonly Button SettingsBtn;


        public RenderedFrame(VectorizedFrame frame)
        {
            // Saving the frame, so I can delete it later from the image
            Frame = frame;

            // Rendering the frame onto an image control
            Image renderedFrame = new Image();
            renderedFrame.Source = frame.GetRenderedFrame();
            RenderOptions.SetBitmapScalingMode(renderedFrame, BitmapScalingMode.Fant);
            Children.Add(renderedFrame);

            DeleteBtn = CreateDeleteBtn();
            SettingsBtn = CreateSettingsBtn();

            SpeedBtnPanel = new SpeedBtnPanel(Frame);

            // Adding them to the frame
            Children.Add(DeleteBtn);
            Children.Add(SettingsBtn);
            Children.Add(SpeedBtnPanel);
        }

        Button CreateDeleteBtn()

        {
            // Creating the delete button
            Button deleteBtn = new Button()
            {
                Content = new Image()
                {
                    Source = AssetManager.GetBmpFrame("ButtonImages/Delete.png"),
                    Margin = new Thickness(3)
                },
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Opacity = 0,
                Style = (Style)Application.Current.FindResource("SmallButton")
            };

            deleteBtn.Click += (o, e)
                => DeleteFrame();

            return deleteBtn;
        }

        Button CreateSettingsBtn()
        {
            // Creating the settings button
            Button settingsBtn = new Button()
            {
                Content = new Image()
                {
                    Source = AssetManager.GetBmpFrame("ButtonImages/Settings.png"),
                    Margin = new Thickness(3)
                },
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Opacity = 0,
                Style = (Style)Application.Current.FindResource("SmallButton")
            };

            settingsBtn.Click += (o, e)
                => SpeedBtnPanel.FadeIn();

            return settingsBtn;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            // If the mouse enters the frame, the buttons fade in 
            AnimationPlayer.FadeIn(DeleteBtn);
            AnimationPlayer.FadeIn(SettingsBtn);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // If the mouse leaves the frame, the buttons fade out
            AnimationPlayer.FadeOut(DeleteBtn);
            AnimationPlayer.FadeOut(SettingsBtn);
            SpeedBtnPanel.FadeOut();
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
            ("6f", 6),
            ("12f", 12),
            ("24f", 24)
        };

        static readonly Brush SELECTED_BORDER_BRUSH = Brushes.LightBlue;
        static readonly Brush UNSELECTED_BORDER_BRUSH = Brushes.Black;

        public bool IsCollapsed => Visibility == Visibility.Collapsed;

        // Referencing it here, so I can alter how many times the image has to be replayed
        VectorizedFrame Frame;

        public SpeedBtnPanel(VectorizedFrame frame)
        {
            Frame = frame;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0, 31, 0, 0);
            Visibility = Visibility.Collapsed;
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
                    FadeOut();
                };

                Children.Add(btn);
            }
        }

        public void FadeIn()
            => AnimationPlayer.FadeIn(this);

        public void FadeOut()
            => AnimationPlayer.FadeOut(this);
    }
}
