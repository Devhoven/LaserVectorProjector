using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectorInterface
{
    public partial class MainWindow : Window
    {
        Point beginning = new Point();
        int childCount = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                    beginning = e.GetPosition(Canvas);
            } 
            else if (e.ChangedButton == MouseButton.Right)
            {
                RemoveChilds(1);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point CurrentMousePos = e.GetPosition(Canvas);
                RemoveChilds(childCount);

                // Renderoptions:
                // RenderLine(beginning, CurrentMousePos);
                (double X, double Y, double Width, double Height) = GetRectangle(beginning, CurrentMousePos);
                //RenderRectangle(X, Y, Width, Height);
                RenderCircle(beginning, CurrentMousePos);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            childCount++;
        }

        (double, double, double, double) GetRectangle(Point p1, Point p2)
        {
            double Width = p2.X - p1.X;
            double Height = p2.Y - p1.Y;
            if (Width < 0)
                p1.X = p2.X;
            if (Height < 0)
                p1.Y = p2.Y;

            return (p1.X, p1.Y, Math.Abs(Width), Math.Abs(Height));
        }

        void RenderCircle(Point p1, Point p2)
        {
            double radius = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            Ellipse Circ = new Ellipse()
            {
                Width = radius,
                Height = radius,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 4
            };

            RenderOptions.SetEdgeMode(Circ, EdgeMode.Aliased);
            Canvas.Children.Add(Circ);
            childCount = 1;
        }

        void RenderRectangle(double X, double Y, double Width, double Height)
        {
            if (Width == 0 && Height == 0)
                return;

            Rectangle Rect = new Rectangle()
            {
                Width = Width,
                Height = Height,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 4
            };

            RenderOptions.SetEdgeMode(Rect, EdgeMode.Aliased);
            Canvas.Children.Add(Rect);
            childCount = 1;
        }



        private void RenderLine(Point start, Point end)
        {
            Line line = new Line() {
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Red),
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
            };

            RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
            Canvas.Children.Add(line);
            childCount = 1;
        }

        void RemoveChilds(int count)
        {
            Canvas.Children.RemoveRange(childCount-1, count);
            childCount -= count;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Devhoven/LaserVectorProjector",
                UseShellExecute = true
            });
        }

        /*
        void ToggleItemVisibillity(MenuItem Sender, UIElement Element, string ValName)
        {
            if (Element.Visibility == Visibility.Visible)
            {
                Element.Visibility = Visibility.Collapsed;
                Sender.Background = Brushes.Transparent;
            }
            else
            {
                Element.Visibility = Visibility.Visible;
                Sender.Background = Brushes.LightSkyBlue;
            }
        }*/
    }
}