using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    //Alphabetical Bubble Sort | O(n^2)
    class BubbleSort : IAlgorithm<string>
    {
        public void Execute(string[] array)
        {
            ABCSort(array);
        }

        public void ABCSort(string[] words)
        {
            for (int i = 0; i < words.Length - 1; i++)
            {
                for (int j = i + 1; j < words.Length; j++)
                {
                    if (string.Compare(words[i], words[j], StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        string temp = words[i];
                        words[i] = words[j];
                        words[j] = temp;
                    }
                }
            }
        }
    }
}
