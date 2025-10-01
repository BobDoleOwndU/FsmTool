using MtarTool.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FsmTool.Ak;

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

        public void Import(Stream output, string path,uint soundPacketLength)
        {
            string inputPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

            writer.Write(signature);
            writer.Write(0x10);
            writer.WriteZeros(8);

            var wemPath = inputPath + "_fsm\\" + Path.GetFileNameWithoutExtension(path) + ".wem";
            var wemFile = new WemFile();
            var soundPackets = new Dictionary<double, byte[]>();
            if (File.Exists(wemPath))
            {
                var reader = new BinaryReader(new FileStream(wemPath, FileMode.Open));
                wemFile.Read(reader);
                uint position = 0;
                while (position < wemFile.fileSize)
                {
                    //Console.WriteLine($"position: {position}, fileSize={wemFile.fileSize}");
                    var time = wemFile.GetTimeAtPosition(position);
                    reader.BaseStream.Position = position;
                    var packetData = reader.ReadBytes((int)soundPacketLength);
                    soundPackets[time] = packetData;
                    position += soundPacketLength;
                }
                reader.Close();
            }

            var isWemFile = soundPackets.Count>0;
            double lastTime = 0;
            var isWemStarted = false;
            for (int i = 0; i < files.Count(); i++)
            {
                var filePath = inputPath + "_fsm\\" + files[i].name;
                byte[] file = File.ReadAllBytes(filePath);
                if (isWemFile)
                {
                    var reader = new BinaryReader(new FileStream(filePath, FileMode.Open));
                    uint packetType = reader.ReadUInt32();
                    uint packetSize = reader.ReadUInt32();
                    double packetTime = reader.ReadDouble();
                    reader.Close();
                    var previousPacketTime = lastTime;
                    if (packetType == 541347411)
                    {
                        continue;
                    }
                    foreach (var pair in soundPackets)
                    {
                        if (previousPacketTime <= pair.Key && pair.Key < packetTime)
                        {
                            writer.Write(541347411);
                            if (!isWemStarted)
                                writer.Write(pair.Value.Length+0x20);
                            else
                                writer.Write(pair.Value.Length+0x10);
                            writer.Write(pair.Key);
                            if (!isWemStarted)
                            {
                                writer.Write(wemFile.fileSize);
                                writer.Write(2);
                                writer.WriteZeros(8);
                                isWemStarted = true;
                            }
                            writer.Write(pair.Value);
                            break;
                        }
                    }
                    lastTime = packetTime;
                }
                writer.Write(file);
            } //for ends
        } //method Import ends
    } //class FsmFile ends
} //namespace FsmTool.Fsm ends
