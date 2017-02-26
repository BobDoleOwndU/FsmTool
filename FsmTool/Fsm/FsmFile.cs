using MtarTool.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FsmTool.Fsm
{
    [XmlType("FsmFile")]
    public class FsmFile
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlAttribute("Signature")]
        public uint signature;

        [XmlIgnore]
        public uint filesOffset;

        [XmlArray("Entries")]
        public List<FsmSubFile> files = new List<FsmSubFile>(0);

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            filesOffset = reader.ReadUInt32();

            reader.BaseStream.Position = filesOffset;

            while(reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Console.WriteLine(reader.BaseStream.Position.ToString("x"));

                FsmSubFile fsf = new FsmSubFile();

                fsf.Read(input);
                files.Add(fsf);
                reader.BaseStream.Position = fsf.offset + fsf.size;

                if (fsf.extension == ".end")
                    break;
            } //while ends
        } //method Read ends

        public void Export(Stream output, string path)
        {
            string fileName = path.Replace("_fsm", ".fsm");

            Directory.CreateDirectory(path);
            bool firstSnd = true;
            List<byte> wemBytes = new List<byte>(0);

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].name = i.ToString("0000") + files[i].extension;
                
                File.WriteAllBytes(path + "\\" + files[i].name, files[i].ReadData(output));

                if (files[i].extension == ".snd")
                {
                    int readOffset = 16;

                    if (firstSnd)
                    {
                        readOffset = 32;
                        firstSnd = false;
                    } //if ends

                    wemBytes.AddRange(files[i].ReadData(output).SubArray(readOffset));

                } //if ends
                else if (files[i].extension == ".end")
                    if(wemBytes.Count > 0)
                        File.WriteAllBytes(path + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".wem", wemBytes.ToArray());
            } //for ends
        } //method Export ends

        public void Import(Stream output, string path)
        {
            string inputPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

            writer.Write(signature);
            writer.Write(0x10);
            writer.WriteZeros(8);

            for (int i = 0; i < files.Count(); i++)
            {
                byte[] file = File.ReadAllBytes(inputPath + "_fsm\\" + files[i].name);
                writer.Write(file);
            } //for ends
        } //method Import ends
    } //class FsmFile ends
} //namespace FsmTool.Fsm ends
