using System;
using ProjectorInterface.GalvoInterface;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Source = ProjectorInterface.GalvoInterface.AnimationManager.Source;
using System.Windows;

namespace ProjectorInterface.Helper
{
    class PlayPauseButton : Button
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Source), typeof(PlayPauseButton), new PropertyMetadata(Source.AnimationGallery));

        public static readonly DependencyProperty StandardPathProperty =
            DependencyProperty.Register("StandardPath", typeof(string), typeof(PlayPauseButton), new PropertyMetadata(""));

        public Source Source
        {
            get { return (Source)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string StandardImagePath
        {
            get { return (string)GetValue(StandardPathProperty); }
            set { SetValue(StandardPathProperty, value); }
        }

        public string AlternateImagePath;

        public PlayPauseButton()
        {
            StandardImagePath = "";
            AlternateImagePath = "/Assets/ButtonImages/Pause.png";

            AnimationManager.OnSourceChanged += SourceChanged;
            Click += ToggleAnimation;
        }

        // AnimationChanged Eventhandler, Updates buttonimage when animation has changed
        private void SourceChanged(Source newSource)
        { 
            if(Source == newSource)
                ChangeContent(AlternateImagePath);
            else
                ChangeContent(StandardImagePath);
        }

        // Clickhandler, starts/stops Animation
        public void ToggleAnimation(object sender, RoutedEventArgs e)
        {
            if (AnimationManager.GetAnimation(Source).Running)
            {
                AnimationManager.Stop(Source);
                ChangeContent(StandardImagePath);
            }
            else
            {
                AnimationManager.Start(Source);
            }
        }

        // Changes buttonimage
        private void ChangeContent(string Path)
        {
            Content = new Image
            {
                Source = new BitmapImage(new Uri(Path, UriKind.Relative)),
                Margin = new Thickness(3)
            };
        }
    }
}
