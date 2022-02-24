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
        // Defines the size of the buffer in bytes
        // The first two bytes are for the position (between 0 and 4100, normally) 
        // The fifth byte is for the delay (in microseconds) and the last bit of it is for if the laser is turned on for this line
        const int BUFFER_SIZE = 4;

        // Port object through which the communication is going to be held
        static SerialPort Port;

        public static string PortName => Port == null ? "" : Port.PortName;

        public static bool IsConnected => Port != null && Port.IsOpen;

        static byte[] Buffer;

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
                Line currentLine;
                // Looping through all of the lines contained in the current image
                for (int j = 0; j < frame.LineCount; j++)
                {
                    currentLine = frame.Lines[j];

                    // Maps the coordinates from 0 to 4096 to the correct image section
                    short correctedX = (short)((currentLine.X * Settings.IMG_SECTION) + Settings.IMG_OFFSET);
                    short correctedY = (short)((currentLine.Y * Settings.IMG_SECTION) + Settings.IMG_OFFSET);

                    // Writing the x and y coordinates into the buffer 
                    Buffer[0] = (byte)correctedX;
                    Buffer[1] = (byte)(correctedX >> 8);
                    Buffer[2] = (byte)correctedY;
                    Buffer[3] = (byte)(correctedY >> 8);

                    if (currentLine.On)
                        Buffer[3] |= 0x80;

                    // Sending the data
                    Port.Write(Buffer, 0, BUFFER_SIZE);
                }
            }
        }
    }
}