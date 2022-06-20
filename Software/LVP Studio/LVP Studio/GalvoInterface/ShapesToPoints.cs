using LVP_Studio.Helper;
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
        public static VectorizedImage DrawnImage = new VectorizedImage();
        static readonly int CanvasResolution;

        static List<Line> Lines = new List<Line>();

        static ShapesToPoints()
            => CanvasResolution = (int)MainWindow.Instance.DrawCon.ActualWidth;

        public static void CalcFrameFromCanvas()
        {
            if (Lines.Count == 0)
                DrawnImage.AddFrame(VectorizedFrame.InterpolatedFrame(Lines.ToArray()));

            ShapeWrapper[] Shapes = GenWrapper();

            SortShapes(Shapes);

            AddLine(CanvasResolution / 2, CanvasResolution / 2, false);

            for (int i = 0; i < Shapes.Length; i++)
                Shapes[i].AddPoints(AddLine);

            AddLine(CanvasResolution / 2, CanvasResolution / 2, false);
            
            DrawnImage.AddFrame(VectorizedFrame.InterpolatedFrame(Lines.ToArray()));
            Lines.Clear();
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
            Line startLine = new Line(CanvasResolution / 2, CanvasResolution / 2, false);
            Line currentLine = startLine;

            // This loop circles through each shape and gets the shape closest to the start line
            // Then the shape which was closest to the start is going to be swapped with the element at the current index
            // and the new start line is going to be the end line of the closest shape
            for (int i = 0; i < shapes.Length; i++)
            {
                minDist = double.MaxValue;
                minDistIndex = i;

                for (int j = i; j < shapes.Length; j++)
                {
                    currentLine = shapes[j].StartLine;
                    if (Line.GetDistance(startLine, currentLine) < minDist)
                    {
                        minDist = Line.GetDistance(startLine, currentLine);
                        minDistIndex = j;
                    }
                }

                startLine = shapes[minDistIndex].EndLine;

                (shapes[i], shapes[minDistIndex]) = (shapes[minDistIndex], shapes[i]);
            }
        }

        // Adds a new LineSegment towards the normalized point (x,y) with the laserstatus stroke
        static void AddLine(double x, double y, bool stroke)
            => Lines.Add(Line.NormalizedLine((short)x, (short)y, stroke, CanvasResolution, IMG_SECTION_SIZE));
    }
}
