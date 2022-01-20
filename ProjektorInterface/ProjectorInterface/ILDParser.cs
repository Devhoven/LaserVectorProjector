﻿using ProjectorInterface.GalvoInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectorInterface
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

        public static void Parse()
        {
            DirectoryInfo dirInfo = new DirectoryInfo("C:/Users/Vincent/Pictures/LaserShow");

            foreach (var dir in dirInfo.GetFiles())
            {
                try
                {
                    BinaryReader reader = new BinaryReader(new FileStream(dir.FullName, FileMode.Open));

                    CurrentHeader = ReadHeader(reader);

                    for (int i = 0; i < CurrentHeader.FrameCount; i++)
                    {
                        switch (CurrentHeader.FormatCode)
                        {
                            case FormatCode.Coord3DIndexed:
                                SerialManager.AddImg(new VectorizedImage(ReadData(reader, Read3DIndexed).ToArray()));
                                break;
                            case FormatCode.Coord2DIndexed:
                                SerialManager.AddImg(new VectorizedImage(ReadData(reader, Read2DIndexed).ToArray()));
                                break;
                        }
                        CurrentHeader = ReadHeader(reader);
                    }
                }
                catch (Exception ex)
                { }
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
            reader.Skip(3);

            // Code for the format of the file
            // (0 = 3D coordinate section, 1 = 2D coordinate section, 2 = Colour palette section)
            result.FormatCode = (FormatCode)reader.ReadByte();

            // Skipping the bytes from 9 to 24 (This is just the name of the frame and company name)
            reader.Skip(16);

            result.EntryCount = reader.ReadInt16BE();

            result.FrameNumber = reader.ReadInt16BE();

            result.FrameCount = reader.ReadInt16BE();

            // Skipping the last two bytes (Scanner head and Not used)
            reader.Skip(2);

            return result;
        }

        static List<PointF> ReadData(BinaryReader reader, Func<BinaryReader, (bool, PointF)> readFunc)
        {
            List<PointF> result = new List<PointF>();

            bool lastCoord = false;
            PointF newPoint;

            while(!lastCoord)
            {
                (lastCoord, newPoint) = readFunc(reader);
                result.Add(newPoint);
            }

            return result;
        }

        static (bool, PointF) Read3DIndexed(BinaryReader reader)
        {
            // Reading the x and y coord
            short xPos = reader.ReadInt16BE();
            short yPos = reader.ReadInt16BE();

            // Skipping the z-Coordinate, since I don't need it
            reader.Skip(2);

            // The status code contains in the 8th bit if this is the last point
            // and in the 7th bit if the laser should be one or off 
            byte statusCode = reader.ReadByte();

            // Skipping the color index
            reader.Skip(1);

            return (statusCode >> 7 == 1, new PointF(xPos, yPos, statusCode >> 6 == 1));
        }

        static (bool, PointF) Read2DIndexed(BinaryReader reader)
        {
            // Reading the x and y coord
            short xPos = reader.ReadInt16BE();
            short yPos = reader.ReadInt16BE();

            // The status code contains in the 8th bit if this is the last point
            // and in the 7th bit if the laser should be one or off 
            byte statusCode = reader.ReadByte();

            // Skipping the color index
            reader.Skip(1);

            return (statusCode >> 7 == 1, new PointF(xPos, yPos, statusCode >> 6 == 1));
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
            public FormatCode FormatCode;
            public short EntryCount;
            public short FrameNumber;
            public short FrameCount;

            public override string ToString()
            {
                return "Format Code: " + FormatCode + " Entry Count: " + EntryCount + " Frame Number: " + FrameNumber + " Frame Count: " + FrameCount;
            }
        }  
    }
}
