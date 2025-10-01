using System;
using System.IO;

namespace FsmTool.Ak
{
    class WemFile
    {
        private uint SampleRate;
        private AkVorbisSeekTableItem[] SeekTable;
        private long uDataOffset;
        private uint seekTableSize;
        public uint fileSize;

        public void Read(BinaryReader reader)
        {
            uint signature = reader.ReadUInt32();
            if (signature != 1179011410)
            {
                Console.WriteLine($"Signature {signature} is wrong!");
                return;
            }
            uint chunkSize = reader.ReadUInt32();;
            if (chunkSize>reader.BaseStream.Length)
            {
                Console.WriteLine($"File size {reader.BaseStream.Length} is bigger than riff size {chunkSize}!");
                return;
            }

            fileSize = chunkSize + 8;
            uint waveSignature = reader.ReadUInt32();
            if (waveSignature != 1163280727)
            {
                Console.WriteLine($"Wave Signature {waveSignature} is wrong!");
                return;
            }
            uint fmtSignature = reader.ReadUInt32();
            if (fmtSignature != 544501094)
            {
                Console.WriteLine($"Fmt Signature {fmtSignature} is wrong!");
                return;
            }

            uint fmtChunkSize = reader.ReadUInt32();
            long fmtChunkStart = reader.BaseStream.Position;
            ushort formatTag = reader.ReadUInt16();
            if (formatTag != 65535)
            {
                Console.WriteLine($"Format tag {formatTag} is not Vorbis!");
                return;
            }

            ushort channels = reader.ReadUInt16();
            SampleRate = reader.ReadUInt32();
            
            reader.BaseStream.Position += 0x20;
            
            seekTableSize = reader.ReadUInt32();
            if (!(seekTableSize > 0))
            {
                Console.WriteLine($"No seek table! Please use seek table with granularity 4096.");
                return;
            }

            reader.BaseStream.Position = fmtChunkStart + fmtChunkSize;
            uint dataSignature = reader.ReadUInt32();
            if (dataSignature != 1635017060)
            {
                Console.WriteLine($"No data chunk found at {reader.BaseStream.Position}!");
                return;
            }
            uint dataSize = reader.ReadUInt32();

            uDataOffset = reader.BaseStream.Position;
            
            uint uNumSeekTableItems = seekTableSize / 4;
            SeekTable = new AkVorbisSeekTableItem[uNumSeekTableItems];
            uint uCurFileOffset = (uint)(uDataOffset + seekTableSize);
            uint uCurFrameOffset = 0;
            double time = 0;
            for (int index = 0; index < uNumSeekTableItems; index++)
            {
                var TableItem = new AkVorbisSeekTableItem();
                TableItem.Read(reader);
                SeekTable[index] = TableItem;
                uCurFrameOffset += TableItem.uPacketFrameOffset;
                uCurFileOffset += TableItem.uPacketFileOffset;
                time += (double)TableItem.uPacketFrameOffset / SampleRate;
                //Console.WriteLine($"{reader.BaseStream.Position}: {time} seconds");
            }
        }

        public double GetTimeAtPosition(long position)
        {
            uint uCurFileOffset = (uint)(uDataOffset + seekTableSize);
            //uint uCurFrameOffset = 0;
            double time = 0;
            foreach (AkVorbisSeekTableItem TableItem in SeekTable)
            {
                if (uCurFileOffset > position)
                {
                    Console.WriteLine($"GetTimeAtPosition uCurFileOffset: {uCurFileOffset} position: {position}: {time} seconds");
                    return time;
                }
                uCurFileOffset += TableItem.uPacketFileOffset;
                //uCurFrameOffset += TableItem.uPacketFrameOffset;
                time += (double)TableItem.uPacketFrameOffset / SampleRate;
            }
            return time;
        }
    }

    struct AkVorbisSeekTableItem
    {
        public ushort uPacketFrameOffset;
        public ushort uPacketFileOffset;

        public void Read(BinaryReader reader)
        {
            uPacketFrameOffset = reader.ReadUInt16();
            uPacketFileOffset = reader.ReadUInt16();
        }
    }
}