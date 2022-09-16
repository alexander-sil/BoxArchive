using System;
using System.IO;

namespace BoxArchive
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] files = new string[] { "0.INT", "1.INT", "2.INT" };

            byte[][] data = Logic.PrepareData(new FileInfo("konki.pptx"));

            File.WriteAllBytes("0.INT", data[0]);

            if (!Directory.Exists("testdir"))
            {
                Directory.CreateDirectory("testdir");
            }

            Logic.UnpackFiles(File.ReadAllBytes("0.INT"), new DirectoryInfo("testdir"));


        }
    }
}

