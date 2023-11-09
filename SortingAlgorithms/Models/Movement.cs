using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class Movement
    {
        public int IdFrom { get; }
        public int IdTo { get; }
        public List<Element> Elements { get; }
        public string Comment;
        public Movement(List<Element> items, int idFrom, int idTo, string comment = "") 
        {
            Elements = items;
            IdFrom= idFrom;
            IdTo= idTo;
            Comment= comment;
        }
    }
}
