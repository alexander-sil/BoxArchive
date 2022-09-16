using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;

namespace BoxArchive
{
    public class Logic
    {

        private const string START = "STaRT";

        private const string SEPARATOR = "SEParaTOR";

        public static void ProcessArgs(string[] args)
        {
            try
            {
                if (args[0] == "-c")
                {
                    if ((!Directory.Exists("input")) && (!Directory.Exists("input")))
                    {
                        Directory.CreateDirectory("input");
                        Directory.CreateDirectory("canned");

                        DirectoryInfo inputInfo = new DirectoryInfo("input");
                        DirectoryInfo cannedInfo = new DirectoryInfo("canned");

                        inputInfo.Attributes |= FileAttributes.Hidden;
                        cannedInfo.Attributes |= FileAttributes.Hidden;
                    }
                    else if (!Directory.Exists("input"))
                    {
                        Directory.Delete("canned", true);

                        Directory.CreateDirectory("input");
                        Directory.CreateDirectory("canned");

                        DirectoryInfo inputInfo = new DirectoryInfo("input");
                        DirectoryInfo cannedInfo = new DirectoryInfo("canned");

                        inputInfo.Attributes |= FileAttributes.Hidden;
                        cannedInfo.Attributes |= FileAttributes.Hidden;
                    }
                    else if (!Directory.Exists("canned"))
                    {
                        Directory.Delete("input", true);

                        Directory.CreateDirectory("input");
                        Directory.CreateDirectory("canned");

                        DirectoryInfo inputInfo = new DirectoryInfo("input");
                        DirectoryInfo cannedInfo = new DirectoryInfo("canned");

                        inputInfo.Attributes |= FileAttributes.Hidden;
                        cannedInfo.Attributes |= FileAttributes.Hidden;
                    }
                    else
                    {
                        Directory.Delete("input", true);
                        Directory.Delete("canned", true);

                        Directory.CreateDirectory("input");
                        Directory.CreateDirectory("canned");

                        DirectoryInfo inputInfo = new DirectoryInfo("input");
                        DirectoryInfo cannedInfo = new DirectoryInfo("canned");

                        inputInfo.Attributes |= FileAttributes.Hidden;
                        cannedInfo.Attributes |= FileAttributes.Hidden;
                    }

                    CopyFilesToInputDir(args, 1);
                    CanFiles(args.Where(f => (f != "-с") && (f != String.Empty)));
                }
                else if (args[0] == "-d" && args.Length == 2 && File.Exists(args[1]) && args[1].Contains(".BOX"))
                {

                }
                else
                {
                    Console.WriteLine("Предупреждение: указана неверная директива.");
                }
            }
            catch (Exception e)
            {
                ErrorRoutine(e.Message);
            }

        }

        private static void BoxCans(string destFile)
        {
            if (Directory.Exists("canned") && Directory.Exists("input"))
            {
                ZipFile.CreateFromDirectory("canned", "ARCHIVE.zip", CompressionLevel.Optimal, false);
            }



        }

        private static void CopyFilesToInputDir(string[] files, int startPoint)
        {
            try
            {
                for (int i = startPoint; i < files.Length; i++)
                {
                    if (File.Exists(files[i]))
                    {
                        File.Copy(new FileInfo(files[i]).FullName, Path.Combine("input", files[i]), true);
                    }
                }
            }
            catch
            {
                if (Directory.Exists("input")) { Directory.Delete("input", true); }

                if (Directory.Exists("canned")) { Directory.Delete("canned", true); }

                Console.WriteLine("Ошибка: неверный формат пути.");

                Environment.Exit(0);
            }
        }

        private static void CanFiles(IEnumerable<string> intFiles)
        {
            try
            {
                if (Directory.Exists("canned") && Directory.Exists("input"))
                {
                    foreach (string i in intFiles)
                    {
                        CompressFile(Path.Combine("input", i), Path.Combine("canned", $"{i}.CAN"));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorRoutine(e.Message);
            }
        }

        private static void ErrorRoutine(string message)
        {
            Console.WriteLine($"Ошибка. Пожалуйста, свяжитесь с Вашим системным администратором.\nСообщение для отладки: {message}\n");
            Console.WriteLine("Исполнение завершено.");

            Console.ReadKey(true);

            Environment.Exit(0);
        }

        private static void ArrayPush<T>(ref T[] table, T value)
        {
            Array.Resize(ref table, table.Length + 1);
            table[table.Length - 1] = value;
        }


        public static byte[][] PrepareData(params FileInfo[] files)
        {
            try
            {
                Console.WriteLine("Процесс подготовки файлов запущен.");

                byte[][] result = new byte[][] { };

                List<List<byte>> preparedData = new List<List<byte>>();

                byte[] rule = Encoding.Latin1.GetBytes(SEPARATOR);

                byte[] start = Encoding.Latin1.GetBytes(START);

                if ((files.Length % 2) == 0)
                {

                    int j = 0;
                    for (int i = 0; i < (files.Length / 2); i++)
                    {
                        List<byte> dataToPrepare = new List<byte>();
                        List<byte> header = new List<byte>();


                        header.AddRange(Encoding.Latin1.GetBytes($"{0} " + files[j].Name + "\t").Concat(Encoding.Latin1.GetBytes($"{1} " + files[j + 1].Name + "\t")).Cast<byte>());
                        j++;
                        header.AddRange(start);
                        dataToPrepare.AddRange(header);


                        dataToPrepare.AddRange(File.ReadAllBytes(files[i].FullName).Concat(rule).Concat(File.ReadAllBytes(files[i + 1].FullName).Concat(rule)));

                        dataToPrepare.Reverse();

                        preparedData.Add(dataToPrepare);
                        j++;

                    }

                    for (int i = 0; i < preparedData.Count; i++)
                    {
                        ArrayPush(ref result, preparedData[i].ToArray());
                    }


                }
                else if (files.Length == 1)
                {
                    List<byte> finalDataToPrepare = new List<byte>();
                    List<byte> finalHeader = new List<byte>();

                    finalHeader.AddRange(Encoding.Latin1.GetBytes($"{0} " + files[0].Name + "\t").Cast<byte>());


                    finalHeader.AddRange(start);

                    finalDataToPrepare.AddRange(finalHeader);

                    finalDataToPrepare.AddRange(File.ReadAllBytes(files[0].FullName).Concat(rule).ToList());

                    finalDataToPrepare.Reverse();

                    preparedData.Add(finalDataToPrepare);

                    for (int i = 0; i < preparedData.Count; i++)
                    {
                        ArrayPush(ref result, preparedData[i].ToArray());
                    }
                }
                else
                {

                    for (int i = 0; i < (files.Length / 2); i++)
                    {

                        List<byte> dataToPrepare = new List<byte>();
                        List<byte> header = new List<byte>();


                        header.AddRange(Encoding.Latin1.GetBytes($"{0} " + files[i].Name + "\t").Concat(Encoding.Latin1.GetBytes($"{1} " + files[i + 1].Name + "\t")).Cast<byte>());
                        header.AddRange(start);

                        dataToPrepare.AddRange(header);

                        dataToPrepare.AddRange(File.ReadAllBytes(files[i].FullName).Concat(rule).Concat(File.ReadAllBytes(files[i + 1].FullName).Concat(rule)));

                        dataToPrepare.Reverse();


                        preparedData.Add(dataToPrepare);
                    }

                    List<byte> finalDataToPrepare = new List<byte>();
                    List<byte> finalHeader = new List<byte>();

                    finalHeader.AddRange(Encoding.Latin1.GetBytes($"{0} " + files[files.Length - 1].Name + "\t").Cast<byte>());


                    finalHeader.AddRange(start);

                    finalDataToPrepare.AddRange(finalHeader);

                    finalDataToPrepare.AddRange(File.ReadAllBytes(files[files.Length - 1].FullName).Concat(rule).ToList());

                    finalDataToPrepare.Reverse();

                    preparedData.Add(finalDataToPrepare);

                    for (int i = 0; i < preparedData.Count; i++)
                    {
                        ArrayPush(ref result, preparedData[i].ToArray());
                    }
                }

                Console.WriteLine("Процесс подготовки файлов завершен.");

                return result;



            }
            catch (Exception e)
            {
                ErrorRoutine(e.Message);
                return null;
            }
        }

        public static void CompressFile(string filename, string destfile)
        {
            using FileStream originalFileStream = File.Open(filename, FileMode.Open);
            using FileStream compressedFileStream = File.Create(destfile);
            using var compressor = new GZipStream(compressedFileStream, CompressionLevel.Optimal);
            originalFileStream.CopyTo(compressor);
        }

        public static void DecompressFile(string filename, string destfile)
        {
            using FileStream compressedFileStream = File.Open(filename, FileMode.Open);
            using FileStream outputFileStream = File.Create(destfile);
            using var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
        }

        public static void UnpackWrapper(byte[][] intFiles, DirectoryInfo dir)
        {
            Console.WriteLine("Процесс распаковки файлов запущен.");

            for (int i = 0; i < intFiles.Length; i++)
            {
                UnpackFiles(intFiles[i], dir);
            }

            Console.WriteLine("Процесс распаковки файлов завершен.");
        }

        public static void UnpackFiles(byte[] data, DirectoryInfo dir)
        {
            try
            {

                string prevDir = Directory.GetCurrentDirectory();



                Regex headerSplitter = new Regex(START);
                Regex fileSplitter = new Regex(SEPARATOR);

                data.Reverse();

                string toParse = Encoding.Latin1.GetString(data);

                string[] parsed1 = headerSplitter.Split(toParse);

                string[] parsed2 = fileSplitter.Split(parsed1[1]);

                string[] parsedHeader = parsed1[0].Split('\t').Where(f => f != string.Empty).ToArray();

                Dictionary<int, string> namesAndIndexes = new Dictionary<int, string>();

                for (int i = 0; i < parsedHeader.Length; i++)
                {
                    string[] current = parsedHeader[i].Split(' ');

                    namesAndIndexes.Add(int.Parse(current[0]), current[1]);
                }

                byte[][] parsedData = new byte[][] { };

                Directory.SetCurrentDirectory(prevDir);


                for (int i = 0; i < parsed2.Length; i++)
                {
                    Array.Resize(ref parsedData, parsedData.Length + 1);
                    parsedData[i] = Encoding.Latin1.GetBytes(parsed2[i]);
                }

                parsedData = parsedData.Where(f => f.Length > 0).ToArray();

                for (int i = 0; i < parsedData.Length; i++)
                {
                    if (parsedData.Length == namesAndIndexes.Keys.ToArray().Length)
                    {
                        if (!File.Exists(Path.Combine(dir.Name, namesAndIndexes[i])))
                        {
                            File.Create(Path.Combine(dir.Name, namesAndIndexes[i])).Dispose();
                        }
                        File.WriteAllBytes(Path.Combine(dir.Name, namesAndIndexes[i]), parsedData[i]);
                    }

                }

                Directory.SetCurrentDirectory(prevDir);
            }
            catch (Exception e)
            {
                ErrorRoutine(e.Message);
            }
        }
    }
}
