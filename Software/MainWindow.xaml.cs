using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace ProjectorInterface
{
    public partial class MainWindow : Window
    {
        Point beginning = new Point();
        int childCount = 0;
        int children = 0;
        enum shape { Line, Square, Circle, Triangle }
        shape currentShape = shape.Line;
        bool toggled = false;
        ImageBrush brushline;
        ImageBrush brushsquare;
        ImageBrush brushcircle;
        ImageBrush brushtriangle;

        Line line = new Line()
        {
            StrokeThickness = 4,
            Stroke = new SolidColorBrush(Colors.Red),
            X1 = 0,
            Y1 = 0,
            X2 = 0,
            Y2 = 0
        };

        Ellipse Circle = new Ellipse()
        {
            Width = 0,
            Height = 0,
            Stroke = new SolidColorBrush(Colors.Red),
            StrokeThickness = 4
        };

        Rectangle Rectangle = new Rectangle()
        {
            Width = 0,
            Height = 0,
            Stroke = new SolidColorBrush(Colors.Red),
            StrokeThickness = 4
        };


        public MainWindow()
        {
            InitializeComponent();

            brushline = InitImages("Assets/line.png");
            brushsquare = InitImages("Assets/rectangle.png");
            brushcircle = InitImages("Assets/circle.png");
            brushtriangle = InitImages("Assets/triangle.png");
        }

        // Initialiasing imgaes for when the sidebar is toggled
        private ImageBrush InitImages(string path)
        {
            Uri resourceUri = new Uri(path, UriKind.RelativeOrAbsolute);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;

            return brush;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                beginning = e.GetPosition(Canvas);
                
                if (currentShape == shape.Line)
                {
                    RenderLine(beginning, beginning);
                    Canvas.Children.Add(line);
                }else if(currentShape == shape.Circle)
                {
                    //RenderRectangle(beginning, beginning);
                    Canvas.Children.Add(Circle);
                }else if(currentShape == shape.Square)
                {
                    RenderCircle(beginning, beginning);
                    Canvas.Children.Add(Rectangle);
                }else if(currentShape == shape.Triangle)
                {
                    RenderTriangle(beginning, beginning);
                    Canvas.Children.Add(null);
                }

            }
        }

        // Draws perpetously shapes and immideatly removes them so it seems it only is one shape changing size
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point CurrentMousePos = e.GetPosition(Canvas);

                
                // Renderoptions:
                if (currentShape == shape.Line)
                {
                    RenderLine(beginning, CurrentMousePos);
                }
                else if (currentShape == shape.Square)
                {
                    (double X, double Y, double Width, double Height) = GetRectangle(beginning, CurrentMousePos);
                    RenderRectangle(X, Y, Width, Height);
                }
                else if (currentShape == shape.Circle)
                {
                    RenderCircle(beginning, CurrentMousePos);
                }
                else if (currentShape == shape.Triangle)
                {
                    RenderTriangle(beginning, CurrentMousePos);
                }

                childCount++;
            }
        }

        // Draws the last shape that was being displayed
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            children++;
            //Filler shape that is being removed instead of previously drawn shape
            Canvas.Children.Add(new Line()
            {
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Red),
                X1 = line.X1,
                Y1 = line.Y1,
                X2 = line.X2,
                Y2 = line.Y2
            });
            Canvas.Children.Remove(line);
        }

        // Draws a circle with middle on startig point
        void RenderCircle(Point p1, Point p2)
        {
            double radius = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            Circle.Width = radius * 2;

            Canvas.SetLeft(Circle, beginning.X - radius / 2);
            Canvas.SetTop(Circle, beginning.Y - radius / 2);
        }

        // Draws a rectangle
        void RenderRectangle(double X, double Y, double Width, double Height)
        {
            if (Width == 0 && Height == 0)
                return;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (Width < Height)
                    Height = Width;
                else
                    Width = Height;
            }

            Rectangle.Width = Width;
            Rectangle.Height = Height;
            
            Canvas.SetLeft(Rectangle, X);
            Canvas.SetTop(Rectangle, Y);
        }

        // Calculates the endpoints/width/height of the sqaure with two given points
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

        // Draws a line
        private void RenderLine(Point start, Point end)
        {
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
        }

        // Draws a triangle
        private void RenderTriangle(Point start, Point end)
        {
            Polygon triangle = new Polygon()
            {
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Red),

            };

            Canvas.Children.Add(triangle);
        }

        // Removes all unnecessary drawn shapes and leaves only the one newly drawn
        void RemoveChilds(int count)
        {
            //Canvas.Children.RemoveRange(children, count);
            //childCount = children;
        }

        private void Github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Devhoven/LaserVectorProjector",
                UseShellExecute = true
            });
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            currentShape = shape.Line;
            LineBtn.Background = Brushes.LightSkyBlue;
            SquareBtn.Background = Brushes.Transparent;
            CircleBtn.Background = Brushes.Transparent;
            TriangleBtn.Background = Brushes.Transparent;

        }

        private void Square_Click(object sender, RoutedEventArgs e)
        {
            currentShape = shape.Square;
            LineBtn.Background = Brushes.Transparent;
            SquareBtn.Background = Brushes.LightSkyBlue;
            CircleBtn.Background = Brushes.Transparent;
            TriangleBtn.Background = Brushes.Transparent;
        }

        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            currentShape = shape.Circle;
            LineBtn.Background = Brushes.Transparent;
            SquareBtn.Background = Brushes.Transparent;
            CircleBtn.Background = Brushes.LightSkyBlue;
            TriangleBtn.Background = Brushes.Transparent;
        }

        private void Triangle_Click(object sender, RoutedEventArgs e)
        {
            currentShape = shape.Triangle;
            LineBtn.Background = Brushes.Transparent;
            SquareBtn.Background = Brushes.Transparent;
            CircleBtn.Background = Brushes.Transparent;
            TriangleBtn.Background = Brushes.LightSkyBlue;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            children = 0;
            childCount = 0;

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Toggle Sidebar on/off
        private void Toggle_Click(object sender, RoutedEventArgs e)
        {

            if (toggled)
            {
                toggled = false;
                //LeftCon.Visibility = Visibility.Collapsed;
                LeftCon.Width = 70;
                LineBtn.HorizontalAlignment = HorizontalAlignment.Left;
                SquareBtn.HorizontalAlignment = HorizontalAlignment.Left;
                CircleBtn.HorizontalAlignment = HorizontalAlignment.Left;
                TriangleBtn.HorizontalAlignment = HorizontalAlignment.Left;
                SendBtn.HorizontalAlignment = HorizontalAlignment.Left;



                LineBtn.Background = brushline;
                LineBtn.Content = "";

                SquareBtn.Background = brushsquare;
                SquareBtn.Content = "";

                CircleBtn.Background = brushcircle;
                CircleBtn.Content = "";

                TriangleBtn.Background = brushtriangle;
                TriangleBtn.Content = "";

            }
            else
            {
                toggled = true;
                //LeftCon.Visibility = Visibility.Visible;
                LeftCon.Width = 200;
                LineBtn.HorizontalAlignment = HorizontalAlignment.Center;
                SquareBtn.HorizontalAlignment = HorizontalAlignment.Center;
                CircleBtn.HorizontalAlignment = HorizontalAlignment.Center;
                TriangleBtn.HorizontalAlignment = HorizontalAlignment.Center;
                SendBtn.HorizontalAlignment = HorizontalAlignment.Center;

                LineBtn.Background = Brushes.Transparent;
                LineBtn.Content = "Line";

                SquareBtn.Background = Brushes.Transparent;
                SquareBtn.Content = "Rectangle";

                CircleBtn.Background = Brushes.Transparent;
                CircleBtn.Content = "Circle";

                TriangleBtn.Background = Brushes.Transparent;
                TriangleBtn.Content = "Triangle";
            }
        }
    }
}