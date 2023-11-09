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
            string comment = $"Шаг для сравнения: длина массива / 2 = {vector.Count} / 2 = {step}\n";
            while (step >= 1)
            {
                comment += $"Сравниваем попарно элементы массива с шагом {step} \n";
                for (int i = step; i < vector.Count; i++)
                {
                    int j = i;
                    while (j >= step && vector[j - step].Data > vector[j].Data)
                    {
                        Element temp = vector[j];
                        vector[j] = vector[j - step];
                        vector[j - step] = temp;
                        comment += $"Меняем элементы {vector[j].Data} и {vector[j-step].Data} местами";
                        Movements.Add(new Movement(Element.CopyElements(vector) ,vector[j].Id, vector[j - step].Id, comment));
                        comment = "";
                        j -= step;
                    }
                }
                step = step / 2;
                comment += $"Прошли весь массив, теперь уменьшаем шаг: step / 2 = {step}\n";
            }
        }
    }
}
