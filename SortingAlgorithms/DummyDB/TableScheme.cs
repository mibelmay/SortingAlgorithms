﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace SortingAlgorithms.DummyDB
{
    public class TableScheme
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("columns")]
        public List<Column> Columns { get; set; }

        public static TableScheme ReadFile(string path)
        {
            return JsonSerializer.Deserialize<TableScheme>(File.ReadAllText(path));
        }
    }
}
