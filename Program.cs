using System;
using System.IO;

namespace BoxArchive
{
    class Program
    {
        static void Main(string[] args)
        {
            Logic.ProcessArgs(args);

            Console.WriteLine("Исполнение завершено. Пожалуйста, нажмите любую клавишу для выхода.");
            Console.ReadKey(true);
        }
    }
}

