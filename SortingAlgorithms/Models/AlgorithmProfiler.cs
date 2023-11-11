using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class Result
    {
        public int Lenght { get; set; }
        public string? Name { get; set; }
        public string[]? Sorted { get; set; }
        public double Time { get; set; }
        public Dictionary<string, int>? WordsOfCount { get; set; }


        public void PrintWordsOfCount()
        {
            foreach (var word in WordsOfCount)
            {
                Console.WriteLine($"Слово '{word.Key}' встречается {word.Value} раз(а).");
            }
        }
    }

    public class AlgorithmProfiler
    {
        private Stopwatch stopWatch;

        public AlgorithmProfiler()
        {
            stopWatch = new Stopwatch();
        }

        public void Run(string[] words, IAlgorithm<string> algorithm)
        {
            stopWatch.Reset();
            stopWatch.Start();
            algorithm.Execute(words);
            stopWatch.Stop();
            Console.WriteLine($"Массив длиной: {words.Length} был отсортирован с помощью {algorithm} за {stopWatch.Elapsed.TotalSeconds}");

        }

        public Result RunExtra(string[] words, IAlgorithm<string> algorithm)
        {
            stopWatch.Reset();
            stopWatch.Start();
            algorithm.Execute(words);
            stopWatch.Stop();

            var dictionary = Count(words);

            return new Result
            {
                Lenght = words.Length,
                Name = algorithm.ToString(),
                Sorted = words,
                Time = stopWatch.Elapsed.TotalSeconds,
                WordsOfCount = dictionary,
            };

        }

        public Dictionary<string, int>? Count(string[] arrayOfwords)
        {
            if (arrayOfwords == null)
            {
                return null;
            }

            Dictionary<string, int> wordCounts = new Dictionary<string, int>();

            foreach (string word in arrayOfwords)
            {
                if (wordCounts.ContainsKey(word.ToLower()))
                {
                    wordCounts[word.ToLower()] += 1;
                }
                else
                {
                    wordCounts[word.ToLower()] = 1;
                }
            }
            return wordCounts;
        }
    }
}
