using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.Serialization;

namespace Eve.Yaml.To.Json
{
    internal class Program
    {
        private static List<FileInfo> _YamlFiles = new List<FileInfo>();
        private static string _SourceFolder = "yaml";
        private static string _TargetFolder = "json";

        private static void Main(string[] args)
        {
            DirectoryInfo sdeDir = SdeExists();
            if (sdeDir == null)
            {
                Console.WriteLine("SDE Daten exisiterien nicht");
                Console.Read();
                return;
            }

            //Alle Dateien zusammensuchen
            ListAllFiles(sdeDir);

            //Die gefunden Dateien einfach mal in eine Datei wegschreiben.
            WriteFileNamesInFile(_YamlFiles);

            int count = 1;
            int max = _YamlFiles.Count;

            foreach (FileInfo yamlFile in _YamlFiles)
            {
                Console.WriteLine($"{count++}/{max} -> {yamlFile.FullName}");

                //Das eigentliche konvertieren
                ConvertYamlToJson(yamlFile);
            }

            Console.WriteLine("Fertig. Enter drücken ....");
            Console.Read();
        }

        /// <summary>
        /// Konvertiert die YAML Datei in eine JSON Datei.
        /// </summary>
        /// <param name="yamlFile">YAML Datei</param>
        private static void ConvertYamlToJson(FileInfo yamlFile)
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();

            using (StreamReader sr = new StreamReader(yamlFile.OpenRead()))
            {
                DirectoryInfo targetDir = new DirectoryInfo(yamlFile.DirectoryName.Replace(_SourceFolder, _TargetFolder));
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

        /// <summary>
        /// Schreibt die gefundenen YAML Dateien in eine Datei. Einfach so.
        /// </summary>
        /// <param name="fileInfos">Liste mit allen gefundenen YAML Dateien</param>
        private static void WriteFileNamesInFile(List<FileInfo> fileInfos)
        {
            using (StreamWriter sw = new StreamWriter($"{_SourceFolder}/_YamlFiles.txt"))
            {
                foreach (FileInfo fileInfo in fileInfos)
                {
                    sw.WriteLine(fileInfo.FullName);
                }
            }
        }

        /// <summary>
        /// Erstellt die Liste mit den YAML Dateien rekursiev.
        /// </summary>
        /// <param name="sdeDir">Ordner mit den YAML Dateien</param>
        private static void ListAllFiles(DirectoryInfo sdeDir)
        {
            foreach (FileInfo file in sdeDir.EnumerateFiles())
            {
                _YamlFiles.Add(file);
            }

            foreach (DirectoryInfo directory in sdeDir.EnumerateDirectories())
            {
                ListAllFiles(directory);
            }
        }

        /// <summary>
        /// Prüft mal grundsätzlich ob die SDE Dateien vorhanden sind bzw. der Ordner dafür.
        /// </summary>
        /// <returns>root Ordner für die YAML Dateien oder null wenn er nicht vorhanden war.</returns>
        private static DirectoryInfo SdeExists()
        {
            DirectoryInfo diSource = new DirectoryInfo(_SourceFolder);
            DirectoryInfo diTarget = new DirectoryInfo(_TargetFolder);

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
