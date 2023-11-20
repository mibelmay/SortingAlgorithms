using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SortingAlgorithms.Models
{
    public static class Parser
    {
        public static List<Element> LoadFromFile(string path)
        {
            List<Element> list = new List<Element>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                int index = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!int.TryParse(line, out int element) || element < -100 || element > 100)
                    {
                        throw new Exception("Файл не содержит массив целых чисел");
                    }
                    list.Add(new Element(index, element));
                    index++;
                }
            }
            return list;
        }

        public static string LoadText(string path)
        {
            string text = File.ReadAllText(path);
            return text;
        }

        public static List<Element> GenerateVector()
        {
            List<Element> vector = new List<Element>();
            Random random = new Random();
            int element;
            for (int i = 0; i < 46; i++)
            {
                element = random.Next(1, 101);
                vector.Add(new Element(i, element));
            }
            return vector;
        }
    }
}
