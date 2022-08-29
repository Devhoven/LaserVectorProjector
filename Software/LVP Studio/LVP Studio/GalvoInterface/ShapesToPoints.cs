using System;
using System.Windows;
using LVP_Studio.Helper;
using System.Windows.Shapes;
using System.Collections.Generic;
using static LvpStudio.DrawingCommands.CanvasCommand;
using static LvpStudio.Helper.Settings;
using Rectangle = System.Windows.Shapes.Rectangle;
using WPFLine = System.Windows.Shapes.Line;

namespace LvpStudio.GalvoInterface
{
    static class ShapesToPoints
    {
        static readonly int CANVAS_RESOLUTION = (int)MainWindow.Instance.DrawCon.ActualWidth;
        static readonly Point START_POINT = new Point(CANVAS_RESOLUTION / 2, CANVAS_RESOLUTION / 2, false);

        public static readonly VectorizedImage DrawnImage = new VectorizedImage();
        public static readonly List<Point> Points = new List<Point>();

        public static void CalcFrameFromCanvas()
        {
            Points.Clear();
            
            ShapeWrapper[] Shapes = GenWrapper();

            // Adding an empty frame if there aren't any shapes drawn by the user
            if (Shapes.Length == 0)
            {
                DrawnImage.AddFrame(new VectorizedFrame(Array.Empty<Point>()));
                return;
            }

            SortShapes(Shapes);

            // The laser has to start in the middle of the frame
            AddLine(START_POINT.X, START_POINT.Y, false);

            // Adding all of the lines from the shapes 
            for (int i = 0; i < Shapes.Length; i++)
                Shapes[i].AddPoints(AddLine);

            // The laser has to end in the middle of the frame
            AddLine(START_POINT.X, START_POINT.Y, false);

            DrawnImage.AddFrame(VectorizedFrame.InterpolatedFrame(Points.ToArray()));
        }

        // Generates an array of ShapeWrappers based on the type of each canvas child
        static ShapeWrapper[] GenWrapper()
        {
            List<ShapeWrapper> shapes = new List<ShapeWrapper>();
            foreach (UIElement child in Parent.Children)
            {
                switch (child)
                {
                    case WPFLine line:
                        shapes.Add(new LineWrapper(line));
                        break;
                    case Rectangle rect:
                        shapes.Add(new RectangleWrapper(rect));
                        break;
                    case Ellipse ellipse:
                        shapes.Add(new EllipseWrapper(ellipse));
                        break;
                    case Path path:
                        shapes.Add(new PathWrapper(path));
                        break;
                }
            }   
            return shapes.ToArray();
        }

        // Sorts the shapes depending on the distance from each other
        static void SortShapes(ShapeWrapper[] shapes)
        {
            double currentDist;
            double minDist;
            int minDistIndex;

            // The frame should always start and end in the middle
            Point startPoint = START_POINT;

            // This loop circles through each shape and gets the shape closest to the start line
            // Then the shape which was closest to the start is going to be swapped with the element at the current index
            // and the new start line is going to be the end line of the closest shape
            for (int i = 0; i < shapes.Length; i++)
            {
                minDist = double.MaxValue;
                minDistIndex = i;

                for (int j = i; j < shapes.Length; j++)
                {
                    currentDist = shapes[j].GetShortestDistance(startPoint);
                    if (currentDist < minDist)
                    {
                        minDist = currentDist;
                        minDistIndex = j;
                    }
                }

                startPoint = shapes[minDistIndex].EndPoint;

                (shapes[i], shapes[minDistIndex]) = (shapes[minDistIndex], shapes[i]);
            }
        }

        // Adds a new line segment which gets normalized to the area from 0 to IMG_SECTION_SIZE
        static void AddLine(double x, double y, bool on)
            => Points.Add(Point.NormalizedPoint((short)x, (short)y, on, CANVAS_RESOLUTION, IMG_SECTION_SIZE));
    }
}
