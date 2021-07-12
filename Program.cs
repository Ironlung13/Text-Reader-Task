using System;
using System.IO;

namespace Text_Reader_Task
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid args.");
                ManualReadTextFile();
            }
            else 
            {
                try
                {
                    ReadTextFile(args[0]);
                }
                catch
                {
                    Console.WriteLine("Error reading from file. Switching to manual input.");
                    ManualReadTextFile();
                }
            }

            Console.WriteLine("Program finished. Enter anything to exit.");
            Console.ReadLine();
        }
        static void ManualReadTextFile()
        {
            string fileName = string.Empty;
            while (!File.Exists(fileName))
            {
                Console.WriteLine("Error opening file.");
                Console.Write("Enter file name/path: ");
                fileName = Console.ReadLine();
            }
            try
            {
                ReadTextFile(fileName);
            }
            catch
            {
                Console.WriteLine("Error reading from file. Switching to manual input.");
                ManualReadTextFile();
            }
        }
        static void ReadTextFile(string fileName)
        {
            TextAnalyzer analyzer = new(fileName);
            analyzer.SerializeToJson("ResultsJson.json");
        }
    }
}
