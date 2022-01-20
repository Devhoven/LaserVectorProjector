using ProjectorInterface.GalvoInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectorInterface
{
    static class ILDParser
    {
        static readonly byte[] MAGIC_BYTES = Encoding.ASCII.GetBytes("ILDA");

        public static void Parse()
        {
            BinaryReader reader = new BinaryReader(new FileStream("C:/Users/Vincent/Downloads/haloween.ild", FileMode.Open));

            HeaderInfo header = ReadHeader(reader);

            for (int i = 0; i < header.FrameCount; i++)
            {
                SerialManager.AddImg(new VectorizedImage(ReadFormart0(reader).ToArray()));
            }
            SerialManager.Initialize("COM5");
        }

        static HeaderInfo ReadHeader(BinaryReader reader)
        {
            HeaderInfo result = new HeaderInfo();

            // Checking the magic bytes
            for (int i = 0; i < MAGIC_BYTES.Length; i++)
                if (MAGIC_BYTES[i] != reader.ReadByte())
                    throw new Exception("Not a valid .ild file");

            // Skipping bytes 5 - 7
            reader.SkipTo(8);

            // Code for the format of the file
            // (0 = 3D coordinate section, 1 = 2D coordinate section, 2 = Colour palette section)
            result.FormatCode = reader.ReadByte();

            // Skipping the bytes from 9 to 24 (This is just the name of the frame and company name)
            reader.SkipTo(25);

            result.EntryCount = reader.ReadInt16BE();

            result.FrameNumber = reader.ReadInt16BE();

            result.FrameCount = reader.ReadInt16BE();

            // Skipping the last two bytes (Scanner head and Not used)
            reader.Skip(2);

            return result;
        }

        static List<PointF> ReadFormart0(BinaryReader reader)
        {
            List<PointF> result = new List<PointF>();

            bool lastCoord = false;
            short xPos, yPos;
            byte statusCode;

            while(!lastCoord)
            {
                // Reading the x and y coord
                xPos = reader.ReadInt16BE();
                yPos = reader.ReadInt16BE();
                // Skipping the z-Coordinate, since I don't need it
                reader.Skip(2);

                // The status code contains in the 8th bit if this is the last point
                // and in the 7th bit if the laser should be one or off 
                statusCode = reader.ReadByte();
                result.Add(new PointF(xPos, yPos, statusCode >> 6 == 1));
                lastCoord = statusCode >> 7 == 1;

                // Skipping the color index
                reader.Skip(1);
            }

            return result;
        }

        // Skips to the given position
        // The next value that is going to be read is going to be at the given position
        static void SkipTo(this BinaryReader reader, uint pos)
            => reader.BaseStream.Position = pos - 1; // - 1, since its zero basesd

        // Skips 'count' - bytes in the stream
        static void Skip(this BinaryReader reader, uint count)
            => reader.BaseStream.Position += count;

        // A short with Big Endian byte order
        static short ReadInt16BE(this BinaryReader reader)
            => BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray());

        struct HeaderInfo
        {
            public byte FormatCode;
            public short EntryCount;
            public short FrameNumber;
            public short FrameCount;
        }
    }
}
