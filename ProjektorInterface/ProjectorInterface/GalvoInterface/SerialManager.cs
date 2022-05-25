using ProjectorInterface.Helper;
using ProjectorInterface.Helpler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using static ProjectorInterface.Helper.Settings;

namespace ProjectorInterface.GalvoInterface
{
    static class SerialManager
    {
        // Defines the baud rate at which the pc is going to communicate with the arduino (irrelevant for the DUO)
        const int BAUD_RATE = 2_000_000;
        // How many bytes a single point takes up
        // The first two bytes are for the position (between 0 and 4100, normally) 
        // The fifth byte is for the delay (in microseconds) and the last bit of it is for if the laser is turned on for this line
        const int SIZE_PER_POINT = 4;
        // Defines the size of the buffer in bytes
        // Sending two points per packet, since this is the most efficient
        const int BUFFER_SIZE = SIZE_PER_POINT * 2;

        static byte[] Buffer;

        // Port object through which the communication is going to be held
        static SerialPort Port;

        public static string PortName => Port == null ? "" : Port.PortName;
        public static bool IsConnected => Port != null && Port.IsOpen;

        // Saves the last point of the last played frame, so I can interpolate between this one and the new start point
        static Line LastLine = new Line(0, 0, false);

        static SerialManager()
        {
            Port = null!;
            string portName = RegistryManager.GetVal("PortName", string.Empty);
            if (portName != string.Empty)
                Initialize(portName);

            Buffer = new byte[BUFFER_SIZE];

            // Since static desctructors don't exist, I have to do this
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                // Closes the port if it was open at the end of the app
                if (Port != null && Port.IsOpen)
                    Port.Close();
            };
        }

        // Initializes the port 
        public static void Initialize(string portName)
        {
            if (Port != null)
                Port.Close();

            Port = new SerialPort(portName, BAUD_RATE);

            // If a port gets selected that cannot be opened, this would throw an exception
            try
            {
                Port.Open();
            }
            catch { }
        }

        public static void SendFrame(VectorizedFrame frame)
        {
            lock (frame)
            {
                if (frame.LineCount == 0)
                    return;

                count = 0;

                Line currentLine = frame.Lines[0];

                //if (Math.Abs(LastLine.X - currentLine.X) > MAX_STEP_SIZE ||
                //    Math.Abs(LastLine.Y - currentLine.Y) > MAX_STEP_SIZE)
                //    SmoothTransition(ref currentLine);

                // Looping through all of the lines contained in the current image
                for (int i = 1; i < frame.LineCount; )
                {
                    // Completely fills the buffer with points
                    // This is done, so we can send multiple points per write, which is faster
                    for (int bufIndex = 0; bufIndex < BUFFER_SIZE; bufIndex += SIZE_PER_POINT, i++)
                    {
                        // Gets either the current line, or the last line a few times
                        currentLine = frame.Lines[Math.Min(i, frame.LineCount - 1)];

                        FillBuffer(bufIndex, ref currentLine);
                    }

                    // Sending the data
                    Port.Write(Buffer, 0, BUFFER_SIZE);
                }

                LastLine = currentLine;
            }
        }

        // Smoothes the transition from the last line of the last frame to the first line of the new frame
        static void SmoothTransition(ref Line firstLine)
        {
            // Retreiving the start points
            double x = LastLine.X;
            double y = LastLine.Y;

            // Calculating the difference between the old and new coordinates
            double diffX = firstLine.X - x;
            double diffY = firstLine.Y - y;

            // Calculating the manhattan distance
            double dist = Math.Abs(diffX) + Math.Abs(diffY);

            // Ratio between the difference and the manhattan distance
            double xRatio = diffX / dist;
            double yRatio = diffY / dist;

            // Scaling one of the ratios up to 1, (the other one accordingly) so that the galvos move the fastest they possibly can per step
            double mult = 1 / Math.Abs(Math.Abs(xRatio) > Math.Abs(yRatio) ? xRatio : yRatio);
            xRatio *= mult;
            yRatio *= mult;

            Line traversalLine = new Line((short)x, (short)y, false);

            int bufIndex = 0;

            // Traverses from one point to another until the distance is too short and adds the points on the way
            while (true)
            {
                if (Math.Abs(x - firstLine.X) <= MAX_STEP_SIZE && Math.Abs(y - firstLine.Y) <= MAX_STEP_SIZE)
                {
                    traversalLine.X = firstLine.X;
                    traversalLine.Y = firstLine.Y;
                    FillBuffer(bufIndex, ref traversalLine);
                    bufIndex += SIZE_PER_POINT;
                    break;
                }

                x += (MAX_STEP_SIZE * xRatio);
                y += (MAX_STEP_SIZE * yRatio);
                traversalLine.X = (short)x;
                traversalLine.Y = (short)y;
                FillBuffer(bufIndex, ref traversalLine);
                bufIndex += SIZE_PER_POINT;

                if (bufIndex >= BUFFER_SIZE)
                {
                    Port.Write(Buffer, 0, BUFFER_SIZE);
                    bufIndex = 0;
                }
            }

            if (bufIndex < BUFFER_SIZE)
            {
                for (int i = bufIndex; i < BUFFER_SIZE; i += SIZE_PER_POINT)
                    FillBuffer(i, ref LastLine);
            }
            Port.Write(Buffer, 0, BUFFER_SIZE);
        }

        static int count = 0;

        // Converts the coordinates and sends them to the arduino
        static void FillBuffer(int bufIndex, ref Line line)
        {
            // Maps the coordinates from 0 to 4095 to the correct image section
            short correctedX = (short)((line.X * IMG_SECTION) + IMG_OFFSET);
            short correctedY = (short)((line.Y * IMG_SECTION) + IMG_OFFSET);

            // Writing the x and y coordinates into the buffer 
            Buffer[bufIndex] = (byte)correctedX;
            Buffer[bufIndex + 1] = (byte)(correctedX >> 8);
            Buffer[bufIndex + 2] = (byte)correctedY;
            Buffer[bufIndex + 3] = (byte)(correctedY >> 8);

            if (line.On)
                Buffer[bufIndex + 3] |= 0x80;

            count++;
        }
    }
}