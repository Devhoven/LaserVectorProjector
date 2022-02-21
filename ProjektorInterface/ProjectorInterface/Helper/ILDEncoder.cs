using ProjectorInterface.GalvoInterface;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectorInterface.Helper
{
    static class ILDEncoder
    {
        static readonly byte[] MagicBytes = Encoding.ASCII.GetBytes("ILDA");
        // I'm just gonna use the 2d indexed color format
        static readonly byte FormatCode = 1;
        // Has to be 8 bytes long
        // Don't know what I should put in there
        // Just gonna leave it blank
        static readonly byte[] FrameName = Encoding.ASCII.GetBytes("        ");
        // The company name, again, has to be 8 bytes long
        // Fits perfectly
        static readonly byte[] CompanyName = Encoding.ASCII.GetBytes("TH Koeln");

        // For converting to big endian
        static byte[] ConversionBuffer = new byte[2];

        public static void EncodeImg(string path, VectorizedImage img)
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                VectorizedFrame currentFrame;
                for (ushort i = 0; i < img.FrameCount; i++)
                {
                    currentFrame = img.Frames[i];
                    WriteHeader(stream, currentFrame.LineCount, i, img.FrameCount);
                    WriteFrame(stream, currentFrame);
                }
                // The end header
                WriteHeader(stream, 0, img.FrameCount, img.FrameCount);
            }
        }

        static void WriteHeader(Stream stream, ushort entryCount, ushort frameNumber, ushort frameCount)
        {
            // The magic bytes come first
            stream.Write(MagicBytes);
            // Then 3 reserved bytes 
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.WriteByte(0);
            // The format code follows
            stream.WriteByte(FormatCode);
            // After that we have to append the frame and company name
            stream.Write(FrameName);
            stream.Write(CompanyName);
            // Then the number of entries, which is a unsigned 16 bit int
            BinaryPrimitives.WriteUInt16BigEndian(ConversionBuffer, entryCount);
            stream.Write(ConversionBuffer);
            // The index of the frame follows
            BinaryPrimitives.WriteUInt16BigEndian(ConversionBuffer, frameNumber);
            stream.Write(ConversionBuffer);
            // Next is the total number of frames
            BinaryPrimitives.WriteUInt16BigEndian(ConversionBuffer, frameCount);
            stream.Write(ConversionBuffer);
            // And one byte we don't use 
            stream.WriteByte(0);
            // Closing the header is a reserved byte
            stream.WriteByte(0);
        }

        // Writes the data of the given frame into the stream, in the .ild file format
        static void WriteFrame(Stream stream, VectorizedFrame frame)
        {
            Line currentLine;
            for (ushort i = 0; i < frame.LineCount; i++)
            {
                currentLine = frame.Lines[i];
                BinaryPrimitives.WriteInt16BigEndian(ConversionBuffer, TransformCoord(currentLine.X));
                stream.Write(ConversionBuffer);
                // Y is flipped in the .ild file format, dunno why they did it this way
                BinaryPrimitives.WriteInt16BigEndian(ConversionBuffer, (short)(TransformCoord(currentLine.Y) * -1));
                stream.Write(ConversionBuffer);
                byte statusByte = 0;
                // If we are at the last point, we have to set the blanking bit
                if (i == frame.LineCount - 1)
                    statusByte = 1 << 5;
                // If the laser is off, we have to write a 1, if not a 0
                if (!currentLine.On)
                    statusByte |= 1 << 6;
                stream.WriteByte(statusByte);
                // This would be the color index, but we don't have colors yet ;)
                stream.WriteByte(0);

                short TransformCoord(short oldVal)
                    => (short)(((oldVal / (float)Settings.IMG_SECTION_SIZE) - 0.5) * short.MaxValue * 2);
            }
        }
    }
}
