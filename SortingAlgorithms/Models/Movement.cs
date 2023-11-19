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
        public Tuple<Element[], Element[]> MergedArrays { get; }
        public string Comment;
        public Movement(List<Element> items, int idFrom, int idTo, string comment = "", Tuple<Element[], Element[]> mergedArrays = null)
        {
            Elements = items;
            IdFrom = idFrom;
            IdTo = idTo;
            Comment = comment;
            MergedArrays = mergedArrays;
        }
    }
}