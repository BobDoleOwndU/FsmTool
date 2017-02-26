using FsmTool.Fsm;
using System.IO;
using System.Xml.Serialization;

namespace FsmTool
{
    class Program
    {
        static XmlSerializer xmlSerializer = new XmlSerializer(typeof(FsmFile));

        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                string path = Path.GetFullPath(args[0]);

                if (Path.GetExtension(path) == ".fsm")
                    ReadArchive(args[0]);
                else if (Path.GetExtension(path) == ".xml")
                    WriteArchive(args[0]);
            } //if ends
        } //method main ends

        static void ReadArchive(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).Substring(1);
            string outputPath = directory + "\\" + nameWithoutExtension + "_" + extension;
            string xmlPath = path + ".xml";

            using (FileStream input = new FileStream(path, FileMode.Open))
            using (FileStream xmlOutput = new FileStream(xmlPath, FileMode.Create))
            {
                FsmFile file = new FsmFile();

                file.name = Path.GetFileName(path);
                file.Read(input);
                file.Export(input, outputPath);

                xmlSerializer.Serialize(xmlOutput, file);
            } //using ends
        } //method ReadArchive ends

        static void WriteArchive(string path)
        {
            string outputPath = path.Replace(".xml", "");

            using (FileStream xmlInput = new FileStream(path, FileMode.Open))
            using (FileStream output = new FileStream(outputPath, FileMode.Create))
            {
                FsmFile file = xmlSerializer.Deserialize(xmlInput) as FsmFile;

                file.Import(output, outputPath);
            } //using ends
        } //method WriteArchive ends
    } //class Program ends
} //namespace FsmTool ends
