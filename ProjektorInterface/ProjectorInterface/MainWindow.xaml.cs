using ProjectorInterface.Commands;
using System.Collections.Generic;
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

        public MainWindow()
        {
            ILDParser.Parse();

            InitializeComponent();

            // Did this, so the canvas would get the focus of the keyboard
            Loaded += (sender, e) => Keyboard.Focus(DrawCon);
        }

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
    }
}
