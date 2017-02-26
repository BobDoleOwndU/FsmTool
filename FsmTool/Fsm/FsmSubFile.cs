using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FsmTool.Fsm
{
    [XmlType("Entry", Namespace = "Fsm")]
    public class FsmSubFile
    {
        [XmlAttribute("FilePath")]
        public string name;

        [XmlIgnore]
        public string extension;

        [XmlIgnore]
        public long offset;

        [XmlIgnore]
        public uint signature;

        [XmlIgnore]
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
