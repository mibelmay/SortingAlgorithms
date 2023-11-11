using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SortingAlgorithms.Models
{
    public class InsertionSort : IAlgorithm<string>
    {
        public void Execute(string[] array)
        {
            InsertionSortAlgorithm(array);
        }

        public void InsertionSortAlgorithm(string[] array)
        {
            int n = array.Length;
            for (int i = 1; i < n; ++i)
            {
                string key = array[i];
                int j = i - 1;

                while (j >= 0 && string.Compare(array[j], key) > 0)
                {
                    array[j + 1] = array[j];
                    j = j - 1;

                }
                array[j + 1] = key;
            }
        }
    }
}
