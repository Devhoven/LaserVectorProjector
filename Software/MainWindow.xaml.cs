using System;
using System.Collections.Generic;
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

        List<LineSegment> tmp = new List<LineSegment>();
        LineSegment seg = new LineSegment();
        Point currentp = new Point();

        Line line = new Line()
        {
            StrokeThickness = 4,
            Stroke = new SolidColorBrush(Colors.Red),
            X1 = 0,
            Y1 = 0,
            X2 = 0,
            Y2 = 0
        };

        Ellipse circle = new Ellipse()
        {
            Width = 0,
            Height = 0,
            Stroke = new SolidColorBrush(Colors.Red),
            StrokeThickness = 4
        };

        Rectangle rectangle = new Rectangle()
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
                }
                else if (currentShape == shape.Circle)
                {
                    RenderCircle(beginning, beginning);
                    Canvas.Children.Add(circle);
                }
                else if (currentShape == shape.Square)
                {
                    //RenderRectangle(beginning, beginning);
                    Canvas.Children.Add(rectangle);
                }
                else if (currentShape == shape.Triangle)
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
            }
        }

        // Draws the last shape that was being displayed
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            children++;
            switch (currentShape)
            {
                case shape.Line:
                    Canvas.Children.Remove(line);
                    Canvas.Children.Add(new Line
                    {
                        StrokeThickness = 4,
                        Stroke = new SolidColorBrush(Colors.Red),
                        X1 = line.X1,
                        Y1 = line.Y1,
                        X2 = line.X2,
                        Y2 = line.Y2
                    });
                    break;
                case shape.Square:
                    Canvas.Children.Remove(rectangle);
                    Canvas.SetTop(rectangle, beginning.Y);
                    Canvas.SetLeft(rectangle, beginning.X);
                    Canvas.Children.Add(new Rectangle
                    {
                        StrokeThickness = 4,
                        Stroke = new SolidColorBrush(Colors.Red),
                        Width = rectangle.Width,
                        Height = rectangle.Height
                    });
                    break;
                case shape.Circle:
                    Canvas.Children.Remove(circle);
                    Canvas.Children.Add(new Ellipse()
                    {
                        Width = circle.Width,
                        Height = circle.Height,
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = 4
                    });
                    break;
                case shape.Triangle:
                    break;

            }
            childCount++;

            if (childCount > 2)
            {
                List<LineSegment> test = getPoints();
            }
        }

        // Draws a circle with middle on startig point
        void RenderCircle(Point p1, Point p2)
        {
            double radius = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            circle.Width = radius * 2;

            Canvas.SetLeft(circle, beginning.X - radius / 2);
            Canvas.SetTop(circle, beginning.Y - radius / 2);
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

            rectangle.Width = Width;
            rectangle.Height = Height;

            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);
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






        private List<LineSegment> getPoints()
        {


            foreach (Shape child in Canvas.Children)
            {
                if (child is Line)
                {
                    // moving to (X1, Y1) with laser OFF
                    seg.IsStroked = false;
                    currentp.X = ((Line)child).X1;
                    currentp.Y = ((Line)child).Y1;
                    seg.Point = currentp;
                    tmp.Add(seg.Clone());

                    // moving to (X2, Y2) with laser ON
                    seg.IsStroked = true;
                    currentp.X = ((Line)child).X2;
                    currentp.Y = ((Line)child).Y2;
                    seg.Point = currentp;
                    tmp.Add(seg.Clone());
                }
                else if (child is Rectangle)
                {
                    Point p = Canvas.TranslatePoint(new Point(0, 0), child);
                    // moving to "upper left" corner with laser OFF
                    calcCoord(child, false, p.X, p.Y);

                    // "upper right" corner
                    calcCoord(child, true, currentp.X + child.Width, currentp.Y);

                    // "lower right" corner
                    calcCoord(child, true, currentp.X, currentp.Y + child.Height);

                    // "lower left" corner
                    calcCoord(child, true, currentp.X - child.Width, currentp.Y);

                    // back to "upper left"
                    calcCoord(child, true, currentp.X, currentp.Y - child.Height);
                }
                else if (child is Ellipse)
                {

                }
            }
            return tmp;
        }

        private void calcCoord(Shape child, bool stroke, double x, double y)
        {
            seg.IsStroked = stroke;
            currentp.X = x;
            currentp.Y = y;
            seg.Point = currentp;
            tmp.Add(seg.Clone());
        }
    }
}