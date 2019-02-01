using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.Serialization;

namespace Eve.Yaml.To.Json
{
    internal class Program
    {
        private static List<FileInfo> files = new List<FileInfo>();
        private static string sourceFolder = "yaml";
        private static string targetFolder = "json";

        private static void Main(string[] args)
        {
            DirectoryInfo sdeDir = SdeExists();
            if (sdeDir == null)
            {
                Console.WriteLine("SDE Daten exisiterien nicht");
                Console.Read();
                return;
            }

            ListAllFiles(sdeDir);

            WriteFileNamesInFile(files);

            int count = 1;
            int max = files.Count;

            foreach (FileInfo yamlFile in files)
            {
                Console.WriteLine($"{count++}/{max} -> {yamlFile.FullName}");

                ConvertYamlToJson(yamlFile);
            }

            Console.WriteLine("Fertig. Enter drücken ....");
            Console.Read();
        }

        private static void ConvertYamlToJson(FileInfo yamlFile)
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();

            using (StreamReader sr = new StreamReader(yamlFile.OpenRead()))
            {
                DirectoryInfo targetDir = new DirectoryInfo(yamlFile.DirectoryName.Replace(sourceFolder, targetFolder));
                if(targetDir.Exists == false)
                {
                    targetDir.Create();
                }

                using (StreamWriter sw = new StreamWriter($"{targetDir.FullName}/{yamlFile.Name.Replace(yamlFile.Extension, "")}.json"))
                {
                    sw.Write(serializer.Serialize(deserializer.Deserialize(sr)));
                }
            }
        }

        private static void WriteFileNamesInFile(List<FileInfo> fileInfos)
        {
            using (StreamWriter sw = new StreamWriter($"{sourceFolder}/files.txt"))
            {
                foreach (FileInfo fileInfo in fileInfos)
                {
                    sw.WriteLine(fileInfo.FullName);
                }
            }
        }

        private static void ListAllFiles(DirectoryInfo sdeDir)
        {
            foreach (FileInfo file in sdeDir.EnumerateFiles())
            {
                files.Add(file);
                //Console.WriteLine(file.FullName);
            }

            foreach (DirectoryInfo directory in sdeDir.EnumerateDirectories())
            {
                ListAllFiles(directory);
            }

        }

        private static DirectoryInfo SdeExists()
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceFolder);
            DirectoryInfo diTarget = new DirectoryInfo(targetFolder);

            if(diTarget.Exists == false)
            {
                diTarget.Create();
            }

            if (diSource.Exists == false)
            {
                diSource.Create();

                return null;
            }

            return diSource;
        }
    }
}
