using ProjectorInterface.Helper;
using ProjectorInterface.Helpler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ProjectorInterface.GalvoInterface
{
    static class SerialManager
    {
        // Defines the baud rate at which the pc is going to communicate with the arduino
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
                Line currentLine = frame.Lines[0];

                if (Math.Abs(LastLine.X - currentLine.X) > Settings.MAX_STEP_SIZE ||
                    Math.Abs(LastLine.Y - currentLine.Y) > Settings.MAX_STEP_SIZE)
                    SmoothTransition(ref currentLine);

                // Looping through all of the lines contained in the current image
                for (int i = 0; i < frame.LineCount; )
                {
                    // Completely fills the buffer with points
                    // This is done, so we can send multiple points per write, which is faster
                    for (int bufIndex = 0; bufIndex < BUFFER_SIZE; bufIndex += SIZE_PER_POINT, i++)
                    {
                        // Gets either the current line, or the last line a few times
                        currentLine = frame.Lines[Math.Min(i, frame.LineCount - 1)];

                        SendLine(bufIndex, ref currentLine);
                    }
                }
                LastLine = currentLine;
            }
        }

        // Smoothes the transition from the last line of the last frame to the first line of the new frame
        static void SmoothTransition(ref Line firstLine)
        {
            // Calculating the difference between the old and new coordinates
            short diffX = (short)(firstLine.X - LastLine.X);
            short diffY = (short)(firstLine.Y - LastLine.Y);

            // Calculating the manhattan distance
            float dist = MathF.Abs(diffX) + Math.Abs(diffY);

            // Ratio between the difference and the manhattan distance
            float xRatio = diffX / dist;
            float yRatio = diffY / dist;

            // Scaling one of the ratios up to 1, (the other one accordingly) so that the galvos move the fastest they possibly can per step
            float mult = 1 / MathF.Abs(MathF.Abs(xRatio) > MathF.Abs(yRatio) ? xRatio : yRatio);
            xRatio *= mult;
            yRatio *= mult;

            int bufIndex = 0;

            // Traverses from one point to another until the distance is too short and adds the points on the way
            while (Math.Abs(LastLine.X - firstLine.X) > Settings.MAX_STEP_SIZE ||
                   Math.Abs(LastLine.Y - firstLine.Y) > Settings.MAX_STEP_SIZE)
            {
                LastLine.X += (short)(Settings.MAX_STEP_SIZE * xRatio);
                LastLine.Y += (short)(Settings.MAX_STEP_SIZE * yRatio);

                SendLine(bufIndex, ref LastLine);
                bufIndex = (bufIndex + SIZE_PER_POINT) % BUFFER_SIZE;
            }
        }

        // Converts the coordinates and sends them to the arduino
        static void SendLine(int bufIndex, ref Line line)
        {
            // Maps the coordinates from 0 to 4095 to the correct image section
            short correctedX = (short)((line.X * Settings.IMG_SECTION) + Settings.IMG_OFFSET);
            short correctedY = (short)((line.Y * Settings.IMG_SECTION) + Settings.IMG_OFFSET);

            // Writing the x and y coordinates into the buffer 
            Buffer[bufIndex] = (byte)correctedX;
            Buffer[bufIndex + 1] = (byte)(correctedX >> 8);
            Buffer[bufIndex + 2] = (byte)correctedY;
            Buffer[bufIndex + 3] = (byte)(correctedY >> 8);

            if (line.On)
                Buffer[bufIndex + 3] |= 0x80;

            // Sending the data
            Port.Write(Buffer, 0, BUFFER_SIZE);
        }
    }
}