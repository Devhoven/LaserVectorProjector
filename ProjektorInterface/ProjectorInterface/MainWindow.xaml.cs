using System.Windows;
using System.Windows.Input;

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
                Application.Current.Shutdown();
        }
    }
}
