﻿using ProjectorInterface.GalvoInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectorInterface.Helper
{
    static class ILDParser
    {
        static readonly byte[] MAGIC_BYTES = Encoding.ASCII.GetBytes("ILDA");

        enum FormatCode
        {
            Coord3DIndexed,
            Coord2DIndexed,
            ColorPalette,
            Coord3DTrueColor,
            Coord2DTrueColor
        }

        static HeaderInfo CurrentHeader;

        // Loads the frames from the selected file into the given images list
        public static void LoadFromPath(string path, ref List<VectorizedImage> images)
        {
            using BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open));

            CurrentHeader = ReadHeader(reader);

            for (int i = 0; i < CurrentHeader.FrameCount; i++)
            {
                if (CurrentHeader.FormatCode == FormatCode.ColorPalette)
                    ReadColorPalette(reader);
                else
                    images.Add(ReadImgData(reader, ReadDataRecord));

                CurrentHeader = ReadHeader(reader);
            }
        }

        // Reads the next header
        static HeaderInfo ReadHeader(BinaryReader reader)
        {
            HeaderInfo result = new HeaderInfo();

            // Checking the magic bytes
            for (int i = 0; i < MAGIC_BYTES.Length; i++)
                if (MAGIC_BYTES[i] != reader.ReadByte())
                    throw new Exception("Not a valid .ild file");

            // Skipping bytes 5 - 7
            reader.Skip(3);

            // Code for the format of the file
            // (0 = 3D coordinate section, 1 = 2D coordinate section, 2 = Colour palette section)
            result.FormatCode = (FormatCode)reader.ReadByte();

            // Skipping the bytes 9 to 24 (This is just the name of the frame and company name)
            reader.Skip(16);

            result.EntryCount = reader.ReadInt16BE();

            result.FrameNumber = reader.ReadInt16BE();

            result.FrameCount = reader.ReadInt16BE();

            // Skipping the last two bytes (Scanner head and Not used)
            reader.Skip(2);

            return result;
        }

        // Iterates over the current data section and returns an normalized image
        static VectorizedImage ReadImgData(BinaryReader reader, Func<BinaryReader, PointF> readFunc)
        {
            List<PointF> result = new List<PointF>();

            for (int i = 0; i < CurrentHeader.EntryCount; i++)
                result.Add(readFunc(reader));

            return new VectorizedImage(result.ToArray());
        }

        // Is able to read a data record of any frame
        static PointF ReadDataRecord(BinaryReader reader)
        {
            // Reading the x and y coord
            short xPos = reader.ReadInt16BE();
            short yPos = reader.ReadInt16BE();

            // Skipping the z-Coordinate, since I don't need it
            if (CurrentHeader.FormatCode == FormatCode.Coord3DIndexed)
                reader.Skip(2);

            // The status code contains in the 8th bit if this is the last point
            // and in the 7th bit if the laser should be one or off 
            byte statusCode = reader.ReadByte();

            // Skipping the color index
            // Either one byte or two, depending on the current format code
            if (CurrentHeader.FormatCode == FormatCode.Coord3DIndexed || CurrentHeader.FormatCode == FormatCode.Coord2DIndexed)
                reader.Skip(1);
            else
                reader.Skip(3);

            return new PointF(xPos, yPos, (statusCode & 0b01000000) == 64);
        }

        // "Reads" the color palette section
        // Skipping it, since we don't need it yet
        static void ReadColorPalette(BinaryReader reader)
            => reader.Skip((uint)(CurrentHeader.EntryCount * 3));

        // Skips 'count' - bytes in the stream
        static void Skip(this BinaryReader reader, uint count)
            => reader.BaseStream.Position += count;

        // A short with Big Endian byte order
        static short ReadInt16BE(this BinaryReader reader)
            => BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray());

        // Contains the information of each header
        struct HeaderInfo
        {
            public FormatCode FormatCode;
            public short EntryCount;
            public short FrameNumber;
            public short FrameCount;

            public override string ToString()
                => "Format Code: " + FormatCode + " Entry Count: " + EntryCount + " Frame Number: " + FrameNumber + " Frame Count: " + FrameCount;
        }  
    }
}