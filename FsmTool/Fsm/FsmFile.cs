using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsmTool.Fsm
{
    class FsmFile
    {
        private uint signature;
        private uint filesOffset;

        private List<FsmSubFile> files = new List<FsmSubFile>(0);

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
            for(int i = 0; i < files.Count(); i++)
            {
                files[i].name = i.ToString("0000") + files[i].extension;

                Directory.CreateDirectory(path);
                File.WriteAllBytes(path + "\\" + files[i].name, files[i].ReadData(output));
            } //for ends
        } //method Export ends
    } //class FsmFile ends
} //namespace FsmTool.Fsm ends
