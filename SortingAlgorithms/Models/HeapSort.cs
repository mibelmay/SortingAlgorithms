using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    class HeapSort
    {
        public List<Movement> Movements = new List<Movement>();
        private string comment = "";
        private List<Element> sorted = new List<Element>();
        List<Element> inherits = new List<Element>();
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
                comment += "Меняем местами первый и последний элемент";
                Swap(vector, 0, i);
                sorted.Add(vector[i]);
                Heapify(vector, i, 0);
            }
            Movements.Add(new Movement(Element.CopyElements(vector), -1, -1, "", new Tuple<Element[], Element[]> (sorted.ToArray(), inherits.ToArray())));
        }

        void Heapify(List<Element> vector, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            bool isLeft = true;
            if(left >= n && right >= n) { return; }
            inherits.Clear();
            if (left < n) { inherits.Add(vector[left]); } if (right < n) { inherits.Add(vector[right]); }
            Movements.Add(new Movement(Element.CopyElements(vector), vector[largest].Id, -1, $"Сравниваем {vector[largest].Data} с его побочными элементами", new Tuple<Element[], Element[]>(sorted.ToArray(), Element.CopyElements(inherits).ToArray())));
            if (left < n && vector[left].Data > vector[largest].Data)
                largest = left;

            if (right < n && vector[right].Data > vector[largest].Data)
            {
                largest = right;
                isLeft= false;
            }
                

            if (largest != i)
            {
                MakeComment(i, left, right, n, isLeft, vector);
                Swap(vector, largest, i);
                Heapify(vector, n, largest);
            }

        }

        private void Swap(List<Element> vector, int i, int j)
        {
            Movements.Add(new Movement(Element.CopyElements(vector), vector[j].Id, vector[i].Id, comment, new Tuple<Element[], Element[]>(sorted.ToArray(), inherits.ToArray())));
            comment = "";
            Element swap = vector[i];
            vector[i] = vector[j];
            vector[j] = swap;
            Movements.Add(new Movement(Element.CopyElements(vector), vector[j].Id, vector[i].Id, "", new Tuple<Element[], Element[]>(sorted.ToArray(), inherits.ToArray())));
        }

        private void MakeComment(int i, int left, int right, int n, bool isLeft, List<Element> vector)
        {
            if (left < n && right < n && !isLeft) { comment = $"   {vector[i].Data}            {vector[right].Data}" +
                    $"\r\n /   \\   ->   /   \\\r\n{vector[left].Data}   {vector[right].Data}" +
                    $"      {vector[left].Data}   {vector[i].Data}"; 
            }
            else if (left < n && right < n && isLeft) { comment = $"   {vector[i].Data}            {vector[left].Data}" +
                    $"\r\n /   \\   ->   /   \\\r\n{vector[left].Data}   {vector[right].Data}" +
                    $"      {vector[i].Data}   {vector[right].Data}";
            }
            else if( left < n) { comment = $"{vector[i].Data}      {vector[left].Data}\r\n|  ->  |\r\n{vector[left].Data}      {vector[i].Data}"; }
            else if( right < n) { comment = $"{vector[i].Data}      {vector[right].Data}\r\n|  ->  |\r\n{vector[right].Data}      {vector[i].Data}"; }
        }
    }
}
