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
                Console.WriteLine("Failed to get text file name. Switching to manual input.");
                ReadTextFile("War and Peace.txt");
            }
        }

        static void ReadTextFile()
        {
            Console.WriteLine("Enter text file name:");
            string fileName = Console.ReadLine();
            ReadTextFile(fileName);
        }
        static void ReadTextFile(string fileName)
        {
            try
            {
                string[] text = File.ReadAllLines(fileName);
                TextAnalyzer analyzer = new TextAnalyzer(fileName);
                analyzer.WriteAnalysisToFile("Result.txt");
                analyzer.SerializeToJson("ResultsJson.json");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File {fileName} not found.");
                ReadTextFile();
            }
            catch
            {
                Console.WriteLine($"Error getting file.");
            }
        }
    }
}
