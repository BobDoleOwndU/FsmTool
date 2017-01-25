using FsmTool.Fsm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsmTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                string path = Path.GetFullPath(args[0]);

                if(Path.GetExtension(path) == ".fsm")
                {
                    ReadArchive(args[0]);
                } //if ends
            } //if ends
        } //method main ends

        static void ReadArchive(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).Substring(1);
            string outputPath = directory + "\\" + nameWithoutExtension + "_" + extension;

            using (FileStream input = new FileStream(path, FileMode.Open))
            {
                FsmFile file = new FsmFile();
                file.Read(input);
                file.Export(input, outputPath);
            } //using ends
        } //method ReadArchive ends
    } //class Program ends
} //namespace FsmTool ends
