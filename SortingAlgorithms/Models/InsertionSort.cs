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

                comment += $"Сравниваем {key.Data} с элементами, стоящими слева\n";
                while (j >= 0 && input[j].Data > key.Data)
                {
                    comment += $"{input[j].Data} > {key.Data}\n" +
                        $"смещаем {input[j].Data} вправо";
                    Movements.Add(new Movement(Element.CopyElements(input), input[j + 1].Id, input[j].Id, comment));
                    comment = "";

                    input[j + 1] = input[j];
                    Movements.Add(new Movement(Element.CopyElements(input), input[j + 1].Id, input[j].Id));
                    j = j - 1;
                }
                comment += $"Ставим {key.Data} на правильное место";
                Movements.Add(new Movement(Element.CopyElements(input), key.Id, input[j + 1].Id, comment));
                comment = "";
                input[j + 1] = key;
                Movements.Add(new Movement(Element.CopyElements(input), key.Id, input[j + 1].Id));
            }
            Movements.Add(new Movement(Element.CopyElements(input), -1, -1));
        }
    }
}
