using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SortingAlgorithms.DummyDB
{
    public class Column
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; }
        [JsonPropertyName("referencedTable")]
        public string? ReferencedTable { get; set; }
        [JsonPropertyName("referencedColumn")]
        public string? ReferencedColumn { get; set; }

    }
}
