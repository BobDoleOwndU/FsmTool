using System.IO;
using System.Text;

namespace FsmTool.Fsm
{
    class FsmSubFile
    {
        public string name;
        public string extension;

        public long offset;
        private uint signature;
        public int size;

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            offset = reader.BaseStream.Position;
            signature = reader.ReadUInt32();

            if (signature == 0x4F4D4544)
                extension = ".demo";
            else if (signature == 0x20444E53)
                extension = ".snd";
            else if (signature == 0x20444E45)
                extension = ".end";

            size = reader.ReadInt32();
        } //method Read ends

        public byte[] ReadData(Stream input)
        {
            input.Position = offset;
            byte[] data = new byte[size];
            input.Read(data, 0, size);

            return data;
        } //method ReadData ends
    } //class FsmSubFile ends
} //namespace FsmTool.Fsm ends
