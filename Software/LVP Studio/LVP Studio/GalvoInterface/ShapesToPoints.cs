using LVP_Studio.Helper;
using ProjectorInterface.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static ProjectorInterface.DrawingCommands.CanvasCommand;
using static ProjectorInterface.Helper.Settings;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using WPFLine = System.Windows.Shapes.Line;

namespace ProjectorInterface.GalvoInterface
{
    static class ShapesToPoints
    {
        static readonly int CANVAS_RESOLUTION;
        static readonly Point START_POINT;

        public static VectorizedImage DrawnImage = new VectorizedImage();
        static List<Point> Points = new List<Point>();

        static ShapesToPoints()
        {
            CANVAS_RESOLUTION = (int)MainWindow.Instance.DrawCon.ActualWidth;
            START_POINT = new Point(CANVAS_RESOLUTION / 2, CANVAS_RESOLUTION / 2, false);
        }

        public static void CalcFrameFromCanvas()
        {
            // Adding an empty frame
            if (Points.Count == 0)
                DrawnImage.AddFrame(new VectorizedFrame(new Point[0]));

            ShapeWrapper[] Shapes = GenWrapper();

            SortShapes(Shapes);

            // The laser has to start in the middle of the frame
            AddLine(START_POINT.X, START_POINT.Y, false);

            // Adding all of the lines from the shapes 
            for (int i = 0; i < Shapes.Length; i++)
                Shapes[i].AddPoints(AddLine);

            // The laser has to end in the middle of the frame
            AddLine(START_POINT.X, START_POINT.Y, false);
            
            DrawnImage.AddFrame(VectorizedFrame.InterpolatedFrame(Points.ToArray()));
            Points.Clear();
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
            double minDist;
            int minDistIndex;

            // The frame should always start and end in the middle
            Point startPoint = START_POINT;
            Point currentPoint;

            // This loop circles through each shape and gets the shape closest to the start line
            // Then the shape which was closest to the start is going to be swapped with the element at the current index
            // and the new start line is going to be the end line of the closest shape
            for (int i = 0; i < shapes.Length; i++)
            {
                minDist = double.MaxValue;
                minDistIndex = i;

                for (int j = i; j < shapes.Length; j++)
                {
                    currentPoint = shapes[j].StartLine;
                    if (Point.GetDistance(startPoint, currentPoint) < minDist)
                    {
                        minDist = Point.GetDistance(startPoint, currentPoint);
                        minDistIndex = j;
                    }
                }

                startPoint = shapes[minDistIndex].EndLine;

                (shapes[i], shapes[minDistIndex]) = (shapes[minDistIndex], shapes[i]);
            }
        }

        // Adds a new LineSegment towards the normalized point (x,y) with the laserstatus stroke
        static void AddLine(double x, double y, bool stroke)
            => Points.Add(Point.NormalizedPoint((short)x, (short)y, stroke, CANVAS_RESOLUTION, IMG_SECTION_SIZE));
    }
}
