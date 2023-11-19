using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.Models
{
    public class MergedMovement
    {
        public int Id { get; }
        public List<Element> Elements { get; }
        public MergedMovement(int id, List<Element> elements)
        {
            Id = id;
            Elements = elements;
        }
    }
}
