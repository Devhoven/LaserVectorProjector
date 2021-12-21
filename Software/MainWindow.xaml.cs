using System;
using System.Collections.Generic;
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

                RenderLine(beginning, CurrentMousePos);                
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            childCount++;
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
            childCount = childCount - count;
        }
    }
}