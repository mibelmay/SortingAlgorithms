using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class ABCSortAlgorithm : IAlgorithm<string>
    {
        public void Execute(string[] array)
        {
            array = ABCSort(array, 0);
        }

        public string[] ABCSort(string[] array, int rank = 0)
        {
            if (array.Length < 2)
                return array;

            var table = new Dictionary<char, List<string>>();
            var shortWordsCounter = 0;

            foreach (var str in array)
            {
                if (rank < str.Length)
                {
                    var key = char.ToUpper(str[rank]);
                    if (table.ContainsKey(key))
                        table[key].Add(str);
                    else
                        table.Add(key, new List<string> { str });
                }
                else
                {
                    shortWordsCounter++;
                }
            }

            int index = 0;
            for (var i = 'A'; i <= 'Z'; i++)
            {
                if (table.ContainsKey(i))
                {
                    foreach (var str in ABCSort(table[i].ToArray(), rank + 1))
                    {
                        array[index++] = str;
                    }
                }
            }

            return array;
        }
    }
}
