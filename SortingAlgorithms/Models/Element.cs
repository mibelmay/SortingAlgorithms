using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class Element
    {
        public int Id { get; set; }
        public int Data { get; set; }
        public Element(int id, int data)
        {
            Id = id;
            Data = data;
        }

        public static List<Element> CopyElements(List<Element> list)
        {
            List<Element> copy = new List<Element>();
            for (int i = 0; i < list.Count; i++)
            {
                copy.Add(list[i]);
            }
            return copy;
        }
    }
}
