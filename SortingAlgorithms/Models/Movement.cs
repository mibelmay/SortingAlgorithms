using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class Movement
    {
        public int IdFrom { get; set; }
        public int IdTo { get; set; }
        public List<Element> Elements { get; set; }
        //public int[] Elements { get; set; }
        public Movement(List<Element> items, int idFrom, int idTo) 
        {
            Elements = items;
            IdFrom= idFrom;
            IdTo= idTo;
        }
    }
}
