using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    class HeapSort
    {
        public List<Movement> Movements = new List<Movement>();
        public void Execute(List<Element> input)
        {
            HeapSortAlgorithm(input);
        }

        private void HeapSortAlgorithm(List<Element> vector)
        {
            int n = vector.Count;

            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(vector, n, i);

            for (int i = n - 1; i >= 0; i--)
            {
                Swap(vector, 0, i);
                Heapify(vector, i, 0);
            }
        }

        void Heapify(List<Element> vector, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && vector[left].Data > vector[largest].Data)
                largest = left;

            if (right < n && vector[right].Data > vector[largest].Data)
                largest = right;

            if (largest != i)
            {
                Swap(vector, largest, i);
                Heapify(vector, n, largest);
            }
        }

        private void Swap(List<Element> vector, int i, int j)
        {
            Movements.Add(new Movement(Element.CopyElements(vector), vector[j].Id, vector[i].Id));
            Element swap = vector[i];
            vector[i] = vector[j];
            vector[j] = swap;
            Movements.Add(new Movement(Element.CopyElements(vector), vector[j].Id, vector[i].Id));
        }
    }
}
