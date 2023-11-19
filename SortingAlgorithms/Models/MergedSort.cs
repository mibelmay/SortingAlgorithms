using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SortingAlgorithms.Models
{
    public class MergedSort
    {
        private string comment = "";
        public List<Movement> Movements = new List<Movement>();

        public void Execute(List<Element> input)
        {
            MergeSortAlgorithm(input, 0, input.Count - 1);
        }

        public void MergeSortAlgorithm(List<Element> input, int left, int right)
        {
            if (left < right)
            { 
                int mid = left + (right - left) / 2;

                MergeSortAlgorithm(input, left, mid);
                MergeSortAlgorithm(input, mid + 1, right);

                Merge(input, left, mid, right);
            }
            Movements.Add(new Movement(Element.CopyElements(input), -1, -1));
        }

        private void Merge(List<Element> input, int left, int mid, int right)
        {
            comment += $"Рассматриваем подмассивы:\n" +
                $"[{input[left].Data} ; {input[mid].Data}] и [{input[mid+1].Data} ; {input[right].Data}]\n";
            int n1 = mid - left + 1;
            int n2 = right - mid;

            Element[] leftArr = new Element[n1];
            Element[] rightArr = new Element[n2];
            
            for (int i = 0; i < n1; i++)
                leftArr[i] = input[left + i];

            for (int i = 0; i < n2; i++)
                rightArr[i] = input[mid + 1 + i];
            Tuple<Element[], Element[]> tuple = new Tuple<Element[], Element[]>(leftArr, rightArr);
            Movements.Add(new Movement(Element.CopyElements(input), -1, -1, comment, tuple));


            int iLeft = 0, iRight = 0, k = left;

            while (iLeft < n1 && iRight < n2)
            {
                Movements.Add(new Movement(Element.CopyElements(input), leftArr[iLeft].Id, rightArr[iRight].Id, comment + $"Сравниваем {leftArr[iLeft].Data} с {rightArr[iRight].Data}", tuple));
                comment = "";
                if (leftArr[iLeft].Data <= rightArr[iRight].Data)
                {
                    input[k] = leftArr[iLeft];
                    Movements.Add(new Movement(Element.CopyElements(input), leftArr[iLeft].Id, -1, $"Выбираем {leftArr[iLeft].Data}", tuple));
                    iLeft++;
                }
                else
                {
                    input[k] = rightArr[iRight];
                    Movements.Add(new Movement(Element.CopyElements(input), -1, rightArr[iRight].Id, $"Выбираем {rightArr[iRight].Data}", tuple));
                    iRight++;
                }

                k++;
                comment = "";
            }

            while (iLeft < n1)
            {
                input[k] = leftArr[iLeft];
                Movements.Add(new Movement(Element.CopyElements(input), leftArr[iLeft].Id, -1, comment, tuple));
                iLeft++;
                k++;
                comment = ""; 
            }

            while (iRight < n2)
            {
                input[k] = rightArr[iRight];
                Movements.Add(new Movement(Element.CopyElements(input), -1, rightArr[iRight].Id, comment, tuple));
                iRight++;
                k++;
                comment = ""; 
            }
        }
    }
}
