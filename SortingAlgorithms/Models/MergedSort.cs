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
        public List<MergedMovement> NewMovements = new List<MergedMovement>();
        public List<Element> newArray = new List<Element>();
        private List<Element> oldElements= new List<Element>();
        public void Execute(List<Element> input)
        {
            oldElements = Element.CopyElements(input);
            MergeSortAlgorithm(input, 0, input.Count - 1);
        }

        public void MergeSortAlgorithm(List<Element> input, int left, int right)
        {
            oldElements = Element.CopyElements(input);
            if (left < right)
            {
                newArray.Clear();
                int mid = left + (right - left) / 2;

                MergeSortAlgorithm(input, left, mid);
                newArray.Clear();
                MergeSortAlgorithm(input, mid + 1, right);
                newArray.Clear();
                Merge(input, left, mid, right);
                oldElements = Element.CopyElements(input);
            }
            Movements.Add(new Movement(Element.CopyElements(input), -1, -1));
            NewMovements.Add(new MergedMovement(-1, Element.CopyElements(newArray)));
        }

        private void Merge(List<Element> input, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            Element[] leftArr = new Element[n1];
            Element[] rightArr = new Element[n2];
            
            for (int i = 0; i < n1; i++)
                leftArr[i] = input[left + i];

            for (int i = 0; i < n2; i++)
                rightArr[i] = input[mid + 1 + i];
            Tuple<Element[], Element[]> tuple = new Tuple<Element[], Element[]>(leftArr, rightArr);
            Movements.Add(new Movement(Element.CopyElements(oldElements), -1, -1, $"Рассматриваем подмассивы:\n" +
                $"[{input[left].Data} ; {input[mid].Data}] и [{input[mid + 1].Data} ; {input[right].Data}]", tuple));
            NewMovements.Add(new MergedMovement(-1, Element.CopyElements(newArray)));

            int iLeft = 0, iRight = 0, k = left;

            while (iLeft < n1 && iRight < n2)
            {
                Movements.Add(new Movement(Element.CopyElements(oldElements), leftArr[iLeft].Id, rightArr[iRight].Id, "", tuple));
                NewMovements.Add(new MergedMovement(-1, Element.CopyElements(newArray)));
                comment = "";
                if (leftArr[iLeft].Data <= rightArr[iRight].Data)
                {
                    input[k] = leftArr[iLeft];
                    Movements.Add(new Movement(Element.CopyElements(oldElements), leftArr[iLeft].Id, -1, $"{leftArr[iLeft].Data} <= {rightArr[iRight].Data} -> {leftArr[iLeft].Data}", tuple));
                    newArray.Add(leftArr[iLeft]);
                    NewMovements.Add(new MergedMovement(leftArr[iLeft].Id, Element.CopyElements(newArray)));
                    iLeft++;
                }
                else
                {
                    input[k] = rightArr[iRight];
                    Movements.Add(new Movement(Element.CopyElements(oldElements), -1, rightArr[iRight].Id, $"{rightArr[iRight].Data} < {leftArr[iLeft].Data} -> {rightArr[iRight].Data}", tuple));
                    newArray.Add(rightArr[iRight]);
                    NewMovements.Add(new MergedMovement(rightArr[iRight].Id, Element.CopyElements(newArray)));
                    iRight++;
                }

                k++;
            }

            while (iLeft < n1)
            {
                input[k] = leftArr[iLeft];
                Movements.Add(new Movement(Element.CopyElements(oldElements), leftArr[iLeft].Id, -1, $"-> {leftArr[iLeft].Data}", tuple));
                newArray.Add(leftArr[iLeft]);
                NewMovements.Add(new MergedMovement(leftArr[iLeft].Id, Element.CopyElements(newArray)));
                iLeft++;
                k++;
            }

            while (iRight < n2)
            {
                input[k] = rightArr[iRight];
                Movements.Add(new Movement(Element.CopyElements(oldElements), -1, rightArr[iRight].Id, $"-> {rightArr[iRight].Data}", tuple));
                newArray.Add(rightArr[iRight]);
                NewMovements.Add(new MergedMovement(rightArr[iRight].Id, Element.CopyElements(newArray)));
                iRight++;
                k++;
            }
        }
    }
}