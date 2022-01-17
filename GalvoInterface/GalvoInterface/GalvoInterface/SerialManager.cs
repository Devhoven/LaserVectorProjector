using System.Diagnostics;
using System.IO.Ports;

namespace GalvoInteface
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

        // List of all images which are going to be displayed one after another
        static List<VectorizedImage> Images;

        // Seperate thread for sending the image data to the arduino
        static Thread SendImgThread;
        static int CurrentImgIndex;
        static byte[] Buffer;

        static SerialManager()
        {
            Port = null!;
            SendImgThread = null!;

            Images = new List<VectorizedImage>();
            Buffer = new byte[BUFFER_SIZE];
        }

        public static void Initialize(string portName)
        {
            Port = new SerialPort(portName, BAUD_RATE);
            Port.Open();

            CurrentImgIndex = 0;
            SendImgThread = new Thread(new ThreadStart(SendImgLoop));
            SendImgThread.Start();
        }

        public static void AddImg(VectorizedImage img)
            => Images.Add(img);

        public static void RemoveImg(VectorizedImage img)
            => Images.Remove(img);

        public static float Scale = 1f;

        public static float OffsetX = 0;
        public static float OffsetY = 0;

        static void SendImgLoop()
        {
            while (true)
            {
                VectorizedImage currentImg = Images[CurrentImgIndex];
                // Locking the image, since this method runs in a different thread and delete functionality is going to be implemented
                lock (currentImg)
                {
                    // Looping through all of the lines contained in the current image
                    Line currentLine;
                    for (int i = 0; i < currentImg.Lines.Length; i++)
                    {
                        currentLine = currentImg.Lines[i];

                        currentLine.X = (short)((currentLine.X * Scale) + OffsetX);
                        currentLine.Y = (short)((currentLine.Y * Scale) + OffsetY);

                        // Writing the x and y coordinates into the buffer 
                        Buffer[0] = (byte)currentLine.X;
                        Buffer[1] = (byte)(currentLine.X >> 8);
                        Buffer[2] = (byte)currentLine.Y;
                        Buffer[3] = (byte)(currentLine.Y >> 8);

                        //// Writing the delay into the buffer
                        //Buffer[4] = currentLine.Delay;
                        //// The last bit of the delay contains the information if the laser has to be turned off or on on this line
                        //Buffer[4] = (byte)((Buffer[4] & 0b0111111) | (Convert.ToByte(currentLine.On) << 7));

                        // Sending the data
                        Port.Write(Buffer, 0, BUFFER_SIZE);
                    }
                }
                // Moving on to the next image, or looping to the beginning
                CurrentImgIndex = (CurrentImgIndex + 1) % Images.Count;
            }
        }
    }
}