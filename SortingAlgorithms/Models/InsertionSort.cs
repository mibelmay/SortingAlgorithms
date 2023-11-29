using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class InsertionSort
    {
        public List<Movement> Movements = new List<Movement>();

        public void Execute(List<Element> input)
        {
            InsertionSortAlgorithm(input);
        }

        public void InsertionSortAlgorithm(List<Element> input)
        {
            string comment = "";
            for (int i = 1; i < input.Count; i++)
            {
                Element key = input[i];
                int j = i - 1;

                Movements.Add(new Movement(Element.CopyElements(input), -1, key.Id, $"Сравниваем {key.Data} с элементами, стоящими слева"));
                while (j >= 0 && input[j].Data > key.Data)
                {
                    comment += $"{input[j].Data} > {key.Data}\n" +
                        $"смещаем {key.Data} влево";
                    Movements.Add(new Movement(Element.CopyElements(input), input[j + 1].Id, input[j].Id, comment));
                    comment = "";
                    Swap(input, j, j + 1);
                    Movements.Add(new Movement(Element.CopyElements(input), input[j + 1].Id, input[j].Id));
                    j = j - 1;
                }
                Movements.Add(new Movement(Element.CopyElements(input), key.Id, -1, $"{key.Data} встал на правильное место"));
            }
            Movements.Add(new Movement(Element.CopyElements(input), -1, -1));
        }

        private void Swap(List<Element> vector, int i, int j)
        {
            Element temp = vector[i];
            vector[i] = vector[j];
            vector[j] = temp;
        }
    }
}
