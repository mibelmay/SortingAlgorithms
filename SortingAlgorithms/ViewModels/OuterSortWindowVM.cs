using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SortingAlgorithms.Models;
using SortingAlgorithms.DummyDB;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO.Packaging;
using System.Threading;

namespace SortingAlgorithms.ViewModels
{
    class OuterSortWindowVM : ViewModel
    {
        private string _folderPath = "";
        private TableScheme _scheme;
        private Table _table;
        public List<string> SortNames { get; set; } = new List<string>() { "Прямое слияние" };
        private string _selectedSort;
        public string SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                _selectedSort = value;
                OnPropertyChanged();
            }
        }
        private DataTable _dataTable;
        public DataTable DataTable
        {
            get { return _dataTable; }
            set
            {
                _dataTable = value;
                OnPropertyChanged();
            }
        }
        private DataTable _dataTableA = new DataTable();
        public DataTable DataTableA
        {
            get { return _dataTableA; }
            set
            {
                _dataTableA = value;
                OnPropertyChanged();
            }
        }
        private DataTable _dataTableB = new DataTable();
        public DataTable DataTableB
        {
            get { return _dataTableB; }
            set
            {
                _dataTableB = value;
                OnPropertyChanged();
            }
        }
        private List<string> _columnNames;
        public List<string> ColumnNames
        {
            get { return _columnNames; }
            set
            {
                _columnNames = value; OnPropertyChanged();
            }
        }
        private string _selectedColumn;
        public string SelectedColumn
        {
            get { return _selectedColumn; } 
            set
            {
                _selectedColumn = value;
                OnPropertyChanged();
            }
        }

        //для сортировки
        private int _iterations = 1;
        private int _segments;

        public ICommand Sort => new CommandDelegate(param =>
        {
            SelectSort();
        });
        private void SelectSort()
        {
            switch (SelectedSort)
            {
                case "Прямое слияние":
                    DoDirectMerge();
                    break;
            }
        }

        public ICommand Load => new CommandDelegate(param =>
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                _folderPath = openFolderDialog.SelectedPath;
            }
            _scheme = TableReader.LoadScheme(_folderPath);
            _table = TableReader.LoadTable(_scheme, _folderPath);
            ColumnNames = TableReader.CreateColumnNamesList(_scheme);
            DataTable = CreateDataTable(_scheme, _table);
        });

        private DataTable CreateDataTable(TableScheme scheme, Table table)
        {
            DataTable dataTable = new DataTable();
            TableReader.AddColumnsToDataTable(scheme.Columns, dataTable);
            TableReader.AddRowsToDataTable(table.Rows, dataTable);
            return dataTable;
        }

        private void DoDirectMerge() //добавляет колонки во вспомогательные таблицы
        {
            _iterations = 1;
            _segments = 1;
            DataTable dataTableA = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableA.Columns.Add(column.Name);
            }
            DataTableA.Rows.Clear();
            DataTableA = dataTableA;
            DataTable dataTableB = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableB.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTableB;

            DoDirectSort();
        }

        async void DoDirectSort()
        {
            while (true)
            {
                _segments = 1;
                int counter = 0;
                bool flag = true;
                int counter1 = 0;
                foreach (DataRow row in DataTable.Rows)
                {
                    // если достигли количества элементов в последовательности -
                    // меняем флаг для след. файла и обнуляем счетчик количества
                    if (counter == _iterations)
                    {
                        flag = !flag;
                        counter = 0;
                        _segments++;
                    }
                    if (flag)
                    {

                        AddRowInTable(row, DataTableA);
                        counter++;
                        await Task.Delay(1000);
                    }
                    else
                    {
                        AddRowInTable(row, DataTableB);
                        counter++;
                        await Task.Delay(1000);
                    }
                    counter1++;

                }
                flag = true;

                if (_segments == 1)
                {
                    break;
                }

                DataTable.Rows.Clear();
                DataRow newRowA = DataTable.NewRow();
                DataRow newRowB = DataTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                bool pickedA = false, pickedB = false, endA = false, endB = false;
                int positionA = 0;
                int positionB = 0;
                int currentPA = 0;
                int currentPB = _iterations;
                while(true)
                {
                    if (endA && endB)
                    {
                        break;
                    }

                    if (counterA == 0 && counterB == 0)
                    {
                        counterA = _iterations;
                        counterB = _iterations;
                    }

                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (counterA > 0)
                        {
                            if (!pickedA)
                            {
                                newRowA = DataTableA.Rows[positionA];
                                positionA += 1;
                                pickedA = true;
                            }
                        }
                    }
                    else
                    {
                        endA = true;
                    }

                    if (positionB != DataTableB.Rows.Count)
                    {
                        if (counterB > 0)
                        {
                            if (!pickedB)
                            {
                                newRowB = DataTableB.Rows[positionB];
                                positionB += 1;
                                pickedB = true;
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }

                    if (endA && endB && pickedA == false && pickedB == false)
                    {
                        break;
                    }
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            DataColumn myColunm = DataTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                            string tempA = string.Format("{0}", newRowA[myColunm.ToString()]);
                            string tempB = string.Format("{0}", newRowB[myColunm.ToString()]);
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                AddRowInTable(newRowA, DataTable);
                                counterA--;
                                pickedA = false;

                                await Task.Delay(1000);
                            }
                            else
                            {
                                AddRowInTable(newRowB, DataTable);
                                counterB--;
                                pickedB = false;
                            }
                        }
                        else
                        {
                            AddRowInTable(newRowA, DataTable);
                            counterA--;
                            pickedA = false;

                            await Task.Delay(1000);
                        }
                    }
                    else if (pickedB)
                    {
                        AddRowInTable(newRowB, DataTable);
                        counterB--;
                        pickedB = false;

                        await Task.Delay(1000);
                    }

                    currentPA += positionA;
                    currentPB += positionB;
                }
                _iterations *= 2; // увеличиваем размер серии в 2 раза
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
            }
            ChangeMainTable();
            TableReader.SaveChangesToCsv(_table, _folderPath);
        }
        private void AddRowInTable(DataRow newRow, DataTable dataTable)
        {
            dataTable.Rows.Add(newRow.ItemArray);
        }
        private void ChangeMainTable()
        {
            for (int i = 0; i < DataTable.Rows.Count; i++)
            {
                for (int j = 0; j < _scheme.Columns.Count; j++)
                {
                    if (i >= _table.Rows.Count)
                    {
                        _table.Rows.Add(new Row() { Data = new Dictionary<Column, object>() });
                    }
                    string data = DataTable.Rows[i][_scheme.Columns[j].Name].ToString();
                    _table.Rows[i].Data[_scheme.Columns[j]] = data;
                }
            }
        }
        private bool CompareDifferentTypes(string tempA, string tempB)
        {
            string type = "";
            foreach (Column column in _scheme.Columns)
            {
                if (column.Name == SelectedColumn)
                {
                    type = column.Type;
                }
            }
            switch (type)
            {
                case "uint":
                    {
                        return CheckDouble(tempA, tempB);
                    }
                case "int":
                    {
                        return CheckDouble(tempA, tempB);
                    }
                case "double":
                    {
                        return CheckDouble(tempA, tempB);
                    }
                case "datetime":
                    {
                        return CheckDateTime(tempA, tempB);
                    }
                case "string":
                    {
                        return CheckString(tempA, tempB);
                    }
                case "bool":
                    {
                        return CheckDouble(tempA, tempB);
                    }
            }
            return true;
        }

        private bool CheckDouble(string tempA, string tempB)
        {
            double tA = double.Parse(tempA);
            double tB = double.Parse(tempB);
            if (tA < tB)
            {
                return true;
            }
            return false;
        }

        private bool CheckDateTime(string tempA, string tempB)
        {
            DateTime tA = DateTime.Parse(tempA);
            DateTime tB = DateTime.Parse(tempB);
            if (tA < tB)
            {
                return true;
            }
            return false;
        }

        private bool CheckString(string tempA, string tempB)
        {
            int b = tempA.CompareTo(tempB);
            if (b == -1)
            {
                return true;
            }
            return false;
        }
    }
}
