using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Text_Reader_Task
{
    public class TextAnalyzer
    {
        private Dictionary<string, int> words = new();
        private Dictionary<char, int> letters = new();
        public string FileName { get; private set; }
        public int Digits { get; private set; }
        public int Numbers { get; private set; }
        public int Lines { get; private set; }
        public int Punctuation { get; private set; }
        public int WordsWithHyphen { get; private set; }

        public TextAnalyzer(string fileName)
        {
            ReadNewText(fileName);
        }
        public void ReadNewText(string fileName)
        {
            ClearAnalyzer();
            this.FileName = fileName;
            using (StreamReader sr = File.OpenText(fileName))
            {
                while(sr.Peek() != -1)
                {
                    AnalyzeLine(sr.ReadLine());
                    Lines++;
                }
            }
            CountLetters();
        }
        public void SerializeToJson(string fileName)
        {
            var sortedInfo = new
            {
                FileName,
                Digits,
                Numbers,
                Lines,
                Punctuation,
                WordsWithHyphen,
                TotalWords = words.Values.Sum(),
                Words = words.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value),
                LongestWord = words.Keys.Where(x => x.Length == words.Keys.Max(y => y.Length)).First(),
                TotalLetters = letters.Values.Sum(),
                Letters = letters.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value)
            };
            //StringBuilder json = new StringBuilder(JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
            StringBuilder json = new (JsonSerializer.Serialize(sortedInfo, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(fileName, json.ToString());
        }
        private void AnalyzeLine(string line)
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
        private void ClearAnalyzer()
        {
            words.Clear();
            letters.Clear();
            FileName = string.Empty;
            Digits = 0;
            Numbers = 0;
            Lines = 0;
            Punctuation = 0;
            WordsWithHyphen = 0;
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
                WordsWithHyphen++;
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
                    Digits++;
                }
                else if (!char.IsWhiteSpace(ch))
                {
                    Punctuation++;
                }
            }

            if (isNumber == true)
                Numbers++;
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
