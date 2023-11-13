using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Documents;

namespace SortingAlgorithms.DummyDB
{
    public class TableReader
    {
        public static Table Read(TableScheme scheme, string path)
        {
            string[] data = File.ReadAllLines(path);
            Table table = new Table();
            table.Rows = new List<Row>();
            table.Scheme = scheme;
            try
            {
                for (int i = 0; i < data.Length; i++)
                {
                    string[] line = data[i].Split(';');

                    if (line.Length != scheme.Columns.Count)
                    {
                        throw new Exception($"В строке {i + 1} неверное количество столбцов");
                    }

                    table.Rows.Add(AddRow(scheme, line, i));
                }
            }
            catch
            {
                return null;
            }
            return table;
        }

        public static Row AddRow(TableScheme scheme, string[] line, int i)
        {
            Row row = new Row();
            for (int j = 0; j < line.Length; j++)
            {
                switch (scheme.Columns[j].Type)
                {
                    case ("int"):
                        {
                            if (int.TryParse(line[j], out int data))
                            {
                                row.Data.Add(scheme.Columns[j], data);
                            }
                            else
                            {
                                throw new Exception($"В сроке {i + 1} в столбце {j + 1} указаны некорректные данные");
                            }
                            break;
                        }
                    case ("uint"):
                        {
                            if (uint.TryParse(line[j], out uint data))
                            {
                                row.Data.Add(scheme.Columns[j], data);
                            }
                            else
                            {
                                throw new Exception($"В сроке {i + 1} в столбце {j + 1} указаны некорректные данные");
                            }
                            break;
                        }
                    case ("datetime"):
                        {
                            if (DateTime.TryParse(line[j], out DateTime data))
                            {
                                row.Data.Add(scheme.Columns[j], data);
                            }
                            else
                            {
                                throw new Exception($"В сроке {i + 1} в столбце {j + 1} указаны некорректные данные");
                            }
                            break;
                        }
                    case ("double"):
                        {
                            if (double.TryParse(line[j], out double data))
                            {
                                row.Data.Add(scheme.Columns[j], data);
                            }
                            else
                            {
                                throw new Exception($"В сроке {i + 1} в столбце {j + 1} указаны некорректные данные");
                            }
                            break;
                        }
                    default:
                        row.Data.Add(scheme.Columns[j], line[j]);
                        break;
                }
            }
            return row;
        }
        public static TableScheme LoadScheme(string folderPath)
        {
            TableScheme scheme;
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                if (file.Contains(".json"))
                {
                    scheme = TableScheme.ReadFile(file);
                    return scheme;
                }
            }
            throw new Exception("Схема не найдена");
        }

        public static Table LoadTable(TableScheme scheme, string folderPath)
        {
            Table table = new Table();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                if (file.Contains(".csv"))
                {
                    table = Read(scheme, file);
                    return table;
                }
            }
            throw new Exception("Таблица не найдена");
        }
        public static void AddColumnsToDataTable(List<Column> columns, DataTable dataTable)
        {
            foreach (Column column in columns)
            {
                dataTable.Columns.Add(column.Name);
            }
        }
        public static void AddRowsToDataTable(List<Row> rows, DataTable dataTable)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow newRow = dataTable.NewRow();
                foreach (var rowPair in rows[i].Data)
                {
                    newRow[rowPair.Key.Name] = rowPair.Value;
                }
                dataTable.Rows.Add(newRow);
            }
        }
        public static List<string> CreateColumnNamesList(TableScheme scheme)
        {
            List<string> columns = new List<string>();
            foreach (Column column in scheme.Columns)
            {
                columns.Add(column.Name);
            }
            return columns;
        }
        public static void SaveChangesToCsv(Table table, string folderPath)
        {
            StringBuilder newFile = new StringBuilder();
            foreach (Row row in table.Rows)
            {
                string newRow = "";
                foreach (Column column in table.Scheme.Columns)
                {
                    newRow = newRow + $"{row.Data[column]};";
                }
                newRow = newRow.Substring(0, newRow.Length - 1);
                newFile.AppendLine(newRow);
            }
            File.WriteAllText(folderPath + $"\\{table.Scheme.Name}.csv", newFile.ToString());
        }

        public static int GetColumnNumber(string columnName, TableScheme scheme)
        {
            int count = 0;
            foreach(Column column in scheme.Columns)
            {
                if(column.Name == columnName)
                {
                    return count;
                }
                count++;
            }
            throw new Exception("Такой колонки нет");
        }

    }
}
