using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Text_Reader_Task
{
    public class TextAnalyzer
    {
        private Dictionary<string, int> words = new Dictionary<string, int>();
        private Dictionary<char, int> letters = new Dictionary<char, int>();
        private int digits = 0;
        private int numbers = 0;
        private int lines = 0;
        private int punctuation = 0;
        private int wordsWithHyphen = 0;

        public void WriteAnalysisToFile(string fileName, string outputFileName)
        {
            ReadText(fileName);
            using StreamWriter sw = File.CreateText(outputFileName);
            sw.Write(GetResults());
        }
        public void ReadText(string fileName)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                while(sr.Peek() != -1)
                {
                    AnalyzeLine(sr.ReadLine());
                    lines++;
                }
            }

            CountLetters();
        }
        public void AnalyzeLine(string line)
        {
            string[] words = Regex.Split(line, @"(\w+[-’']*\w*)").Where(entry => !string.IsNullOrWhiteSpace(entry)).Select(word => word.ToLower()).ToArray();
            foreach(var word in words)
            {
                if (Regex.IsMatch(word, @"[\p{L}\p{Mn}]+"))
                {
                    AnalyzeWord(word);
                }
                else
                {
                    AnalyzeNonWord(word);
                }
            }
        }
        private void AddWordToDictionary(string word)
        {
            if (words.ContainsKey(word))
            {
                words[word]++;
            }
            else
            {
                words.Add(word, 1);
            }
        }
        private void AnalyzeWord(string input)
        {
            if (Regex.IsMatch(input, @"\w+-\w+"))
            {
                wordsWithHyphen++;
                string[] words = Regex.Split(input, @"-");
                foreach(var word in words)
                {
                    AddWordToDictionary(word);
                }
            }
            else
            {
                AddWordToDictionary(input);
            }
        }

        private void AnalyzeNonWord(string input)
        {
            bool isNumber = false;
            foreach (var ch in input)
            {
                if (char.IsDigit(ch))
                {
                    isNumber = true;
                    digits++;
                }
                else if (!char.IsWhiteSpace(ch))
                {
                    punctuation++;
                }
            }

            if (isNumber == true)
                numbers++;
        }

        private string GetResults()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Total letters: {letters.Values.Sum(x => x)}");
            foreach (var letter in letters.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"{letter.Key} : {letter.Value}");
            }
            sb.AppendLine();

            sb.AppendLine($"Total words: {words.Values.Sum(x => x)}");
            foreach (var word in words.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"{word.Key} : {word.Value}");
            }
            sb.AppendLine();

            var longest = words.Keys.Where(x => x.Length == words.Keys.Max(y => y.Length));
            if (longest.Count() > 1)
            {
                sb.AppendLine($"Longest words:");
            }
            else
            {
                sb.AppendLine($"Longest word:");
            }
            foreach (var word in longest)
            {
                sb.AppendLine($"{word}");
            }
            sb.AppendLine();

            sb.AppendLine($"Lines count: {lines}");
            sb.AppendLine($"Digits count: {digits}");
            sb.AppendLine($"Numbers count: {numbers}");
            sb.AppendLine($"Punctuation count: {punctuation}");
            sb.AppendLine($"Words with Hyphen: {wordsWithHyphen}");

            return sb.ToString();
        }

        private void CountLetters()
        {
            foreach(var key in words.Keys)
            {
                foreach(var ch in key)
                {
                    if (char.IsLetter(ch))
                    {
                        if (letters.ContainsKey(ch))
                        {
                            letters[ch]++;
                        }
                        else
                        {
                            letters.Add(ch, 1);
                        }
                    }
                }
            }
        }
    }
}
