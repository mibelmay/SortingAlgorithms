using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    class ShellSort
    {
        public List<Movement> Movements = new List<Movement>();
        public void Execute(List<Element> input)
        {
            ShellSortAlgorithm(input);
        }

        public void ShellSortAlgorithm(List<Element> vector)
        {
            int step = vector.Count / 2;
            while (step >= 1)
            {
                for (int i = step; i < vector.Count; i++)
                {
                    int j = i;
                    while (j >= step && vector[j - step].Data > vector[j].Data)
                    {
                        Element temp = vector[j];
                        vector[j] = vector[j - step];
                        vector[j - step] = temp;

                        Movements.Add(new Movement(Element.CopyElements(vector) ,vector[j].Id, vector[j - step].Id));

                        j -= step;
                    }
                }
                step = step / 2;
            }
        }
    }
}
