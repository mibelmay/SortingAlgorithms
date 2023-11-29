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
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Windows.Controls;
using System.Windows.Media;

namespace SortingAlgorithms.ViewModels
{
    class OuterSortWindowVM : ViewModel
    {
        public ObservableCollection<string> Steps { get; set; } = new ObservableCollection<string>();
        private string _folderPath = "";
        private TableScheme _scheme;
        private Table _table;
        public List<string> SortNames { get; set; } = new List<string>() { "Прямое слияние" , "Естественное слияние", "Трехпутевое слияние"};
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
        private DataTable _dataTableC = new DataTable();
        public DataTable DataTableC
        {
            get { return _dataTableC; }
            set
            {
                _dataTableC = value; OnPropertyChanged();
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
        private int _slider = 1000;
        public int Slider
        {
            get { return _slider; }
            set { _slider = value; OnPropertyChanged();}
        }

        //для сортировки
        private int _iterations = 1;
        private int _segments;
        private int _columnNumber;
        public DataGrid MainDataGrid { get; set; }
        public DataGrid DataGridA { get; set; }
        public DataGrid DataGridB { get; set; }
        public DataGrid DataGridC { get; set; }

        public ICommand Sort => new CommandDelegate(param =>
        {
            if (!Check()) { return; }
            SelectSort();
        });
        private void SelectSort()
        {
            switch (SelectedSort)
            {
                case "Прямое слияние":
                    DoDirectMerge();
                    break;
                case "Естественное слияние":
                    DoNatureMerge();
                    break;
                case "Трехпутевое слияние":
                    DoThreeWayMerge();
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
            if (_folderPath== null || _folderPath == "") { MessageBox.Show("Выберите таблицу"); return; } 
            _scheme = TableReader.LoadScheme(_folderPath);
            _table = TableReader.LoadTable(_scheme, _folderPath);
            ColumnNames = TableReader.CreateColumnNamesList(_scheme);
            ColumnNames.Remove("Seria");
            DataTable = CreateDataTable(_scheme, _table);
            MainDataGrid.Columns[MainDataGrid.Columns.Count - 1].Visibility = System.Windows.Visibility.Hidden;
        });

        private DataTable CreateDataTable(TableScheme scheme, Table table)
        {
            DataTable dataTable = new DataTable();
            TableReader.AddColumnsToDataTable(scheme.Columns, dataTable);
            TableReader.AddRowsToDataTable(table.Rows, dataTable);
            return dataTable;
        }
        private void ColorTable(DataTable dataTable, int index, string seria)
        {
            DataTable table = new DataTable();
            foreach(Column column in _scheme.Columns)
            {
                table.Columns.Add(column.Name);
            }
            foreach(DataRow row in dataTable.Rows)
            {
                table.Rows.Add(row.ItemArray);
            }
            table.Rows[index]["Seria"] = seria;
            dataTable.Rows.Clear();
            foreach(DataRow row in table.Rows)
            {
                dataTable.Rows.Add(row.ItemArray);
            }
        }

        private void DoDirectMerge() //добавляет колонки во вспомогательные таблицы
        {
            Steps.Clear();
            _iterations = 1;
            _segments = 1;
            DataTable dataTableA = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableA.Columns.Add(column.Name);
            }
            DataTableA.Rows.Clear();
            DataTableA = dataTableA;
            DataGridA.Columns[DataGridA.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DataTable dataTableB = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableB.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTableB;
            DataGridB.Columns[DataGridB.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DoDirectSort();
        }

        //Direct Merge
        async void DoDirectSort()
        {
            while (true)
            {
                int seriaCounter = 0;
                string seria = "0";
                _segments = 1;
                int counter = 0;
                bool flag = true;
                int counter1 = 1;
                Steps.Add($"Разделяем исходную таблицу на две таблицы:\n" +
                    $"добавляем элементы с шагом {_iterations}\n");
                foreach (DataRow row in DataTable.Rows)
                {
                    if (counter == _iterations)
                    {
                        flag = !flag;
                        counter = 0;
                        _segments++;
                        seriaCounter++;
                        if (seriaCounter == 2)
                        {
                            seria = (seria == "0") ? "1" : "0";
                            seriaCounter = 0;
                        }
                    }
                    if (flag)
                    {
                        Steps.Add($"Добавляем строку {counter1} в таблицу А\n");
                        AddRowInTable(row, DataTableA, seria);
                        counter++;
                        await Task.Delay(2010 - Slider);
                    }
                    else
                    {
                        Steps.Add($"Добавляем строку {counter1} в таблицу В\n");
                        AddRowInTable(row, DataTableB, seria);
                        counter++;
                        await Task.Delay(2010 - Slider);
                    }
                    counter1++;
                }
                flag = true;

                if (_segments == 1)
                {
                    Steps.Add($"Все элементы оказались в одной таблице\n" +
                        $"Таблица отсортирована по полю {SelectedColumn}");
                    break;
                }

                DataTable.Rows.Clear();
                int counterA = _iterations;
                int counterB = _iterations;
                bool pickedA = false, pickedB = false, endA = false, endB = false;
                int positionA = 0;
                int positionB = 0;
                int currentPA = 0;
                int currentPB = 0;
                string seriaA = "0";
                string seriaB = "0";
                Steps.Add($"Начинаем сливать таблицы А и В\n" +
                    $"Будем сравнивать элементы сериями по {_iterations}\n");
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
                                seriaA = string.Format("{0}", DataTableA.Rows[positionA]["Seria"]);
                                ColorTable(DataTableA, positionA, "2");
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
                                seriaB = string.Format("{0}", DataTableB.Rows[positionB]["Seria"]);
                                ColorTable(DataTableB, positionB, "2");
                                positionB += 1;
                                pickedB = true;
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }
                    await Task.Delay(2010 - Slider);
                    string tempA = string.Format("{0}", DataTableA.Rows[positionA - 1][_columnNumber]);
                    string tempB = string.Format("{0}", DataTableB.Rows[positionB - 1][_columnNumber]);
                    if (endA && endB && pickedA == false && pickedB == false)
                    {
                        break;
                    }
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            Steps.Add($"Сравниваем {tempA} и {tempB}");
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                Steps.Add($"{tempA} < {tempB} \n" +
                                    $"Добавляем {tempA} в таблицу\n");
                                ColorTable(DataTableA, positionA - 1, seriaA);
                                AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                                counterA--;
                                pickedA = false;
                                await Task.Delay(2010 - Slider);
                            }
                            else
                            {
                                Steps.Add($"{tempA} > {tempB} \n" +
                                    $"Добавляем {tempB} в таблицу\n");
                                ColorTable(DataTableB, positionB - 1, seriaB);
                                AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                                counterB--;
                                pickedB = false;
                                await Task.Delay(2010 - Slider);
                            }
                        }
                        else
                        {
                            Steps.Add($"Добавляем оставшуюся строку из А в таблицу\n");
                            ColorTable(DataTableA, positionA - 1, seriaA);
                            AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                            counterA--;
                            pickedA = false;

                            await Task.Delay(2010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        Steps.Add($"Добавляем оставшуюся строку из В в таблицу\n");
                        ColorTable(DataTableB, positionB - 1, seriaB);
                        AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                        counterB--;
                        pickedB = false;

                        await Task.Delay(2010 - Slider);
                    }

                    currentPA += positionA;
                    currentPB += positionB;
                }
                Steps.Add($"Закончили слияние А и В\n" +
                    $"Увеличиваем размер серии: {_iterations} * 2 = {_iterations * 2}\n");
                _iterations *= 2;
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
            }
            ChangeMainTable();
            TableReader.SaveChangesToCsv(_table, _folderPath);
        }
        private void AddRowInTable(DataRow newRow, DataTable dataTable, string seria = null)
        {
            dataTable.Rows.Add(newRow.ItemArray);
            if(seria != null)
            {
                dataTable.Rows[dataTable.Rows.Count - 1]["Seria"] = seria;
            } 
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

        private void DoThreeWayMerge() //добавляет колонки во вспомогательные таблицы
        {
            Steps.Clear();
            _iterations = 1;
            _segments = 1;
            DataTable dataTableA = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableA.Columns.Add(column.Name);
            }
            DataTableA.Rows.Clear();
            DataTableA = dataTableA;
            DataGridA.Columns[DataGridA.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DataTable dataTableB = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableB.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTableB;
            DataGridB.Columns[DataGridB.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DataTable dataTableC = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableC.Columns.Add(column.Name);
            }
            DataTableC.Rows.Clear();
            DataTableC = dataTableC;
            DataGridC.Columns[DataGridC.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DoThreeWaySort();
        }

        //Three-way merge
        async void DoThreeWaySort()
        {
            while (true)
            {
                _segments = 1;
                int countLine = 2;
                string seria = "0";
                int seriaCount = 0;
                int countIterations = 0;
                Steps.Add($"Разделяем исходную таблицу на три таблицы");
                foreach (DataRow row in DataTable.Rows)
                {
                    if(countIterations == _iterations)
                    {
                        Steps.Add($"Mеняем таблицу");
                        _segments++;
                        seriaCount++;
                        if (seriaCount == 3)
                        {
                            seria = (seria == "0") ? "1" : "0";
                            seriaCount = 0;
                        }
                        countIterations = 0;
                    }
                    if (_segments % 3 == 1)
                    {
                        Steps.Add($"Добавляем строку {countLine} в таблицу А");
                        AddRowInTable(row, DataTableA, seria);
                        await Task.Delay(2010 - Slider);
                    }
                    else if (_segments % 3 == 2)
                    {
                        Steps.Add($"Добавляем строку {countLine} в таблицу В");
                        AddRowInTable(row, DataTableB, seria);
                        await Task.Delay(2010 - Slider);
                    }
                    else
                    {
                        Steps.Add($"Добавляем строку {countLine} в таблицу С");
                        AddRowInTable(row, DataTableC, seria);
                        await Task.Delay(2010 - Slider);
                    }
                    countLine++;
                    countIterations++;
                }

                if (_segments == 1)
                {
                    Steps.Add($"Все элементы оказались в одной таблице\n" +
                        $"Таблица отсортирована по полю {SelectedColumn}");
                    break;
                }
                DataTable.Rows.Clear();
                int counterA = _iterations;
                int counterB = _iterations;
                int counterC = _iterations;
                string seriaA = "0";
                string seriaB = "0";
                string seriaC = "0";
                bool pickedA = false, pickedB = false, pickedC = false, endA = false, endB = false, endC = false;
                int positionA = 0; int positionB = 0; int positionC = 0;
                Steps.Add($"Начинаем слияние таблиц А, В и С\n");
                while (true)
                {
                    if (endA && endB && endC && pickedA == false && pickedB == false && pickedC == false)
                    {
                        break;
                    }

                    if (counterA == 0 && counterB == 0 && counterC == 0)
                    {
                        counterA = _iterations;
                        counterB = _iterations;
                        counterC = _iterations;
                    }
                    if (endA)
                    {
                        counterA = 0;
                    }
                    if (endB)
                    {
                        counterB = 0;
                    }
                    if (endC)
                    {
                        counterC = 0;
                    }

                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (counterA > 0)
                        {
                            if (!pickedA)
                            {
                                seriaA = string.Format("{0}", DataTableA.Rows[positionA]["Seria"]);
                                ColorTable(DataTableA, positionA, "2");
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
                                seriaB = string.Format("{0}", DataTableB.Rows[positionB]["Seria"]);
                                ColorTable(DataTableB, positionB, "2");
                                positionB += 1;
                                pickedB = true;
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }

                    if (positionC != DataTableC.Rows.Count)
                    {
                        if (counterC > 0)
                        {
                            if (!pickedC)
                            {
                                seriaC = string.Format("{0}", DataTableC.Rows[positionC]["Seria"]);
                                ColorTable(DataTableC, positionC, "2");
                                positionC += 1;
                                pickedC = true;
                            }
                        }
                    }
                    else
                    {
                        endC = true;
                    }

                    if (endA && endB && endC && pickedA == false && pickedB == false && pickedC == false)
                    {
                        break;
                    }
                    await Task.Delay(2010 - Slider);

                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            string tempA = string.Format("{0}", DataTableA.Rows[positionA - 1][_columnNumber]);
                            string tempB = string.Format("{0}", DataTableB.Rows[positionB - 1][_columnNumber]);
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                if (pickedC)
                                {
                                    string tempC = string.Format("{0}", DataTableC.Rows[positionC - 1][_columnNumber]);
                                    if (CompareDifferentTypes(tempA, tempC))
                                    {
                                        Steps.Add($"{tempA} < {tempC} \n" +
                                    $"Добавляем {tempA} в таблицу\n");
                                        ColorTable(DataTableA, positionA - 1, seriaA);
                                        AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                                        counterA--;
                                        pickedA = false;
                                        await Task.Delay(2010 - Slider);
                                    }
                                    else
                                    {
                                        Steps.Add($"{tempA} > {tempC} \n" +
                                    $"Добавляем {tempC} в таблицу\n");
                                        ColorTable(DataTableC, positionC - 1, seriaC);
                                        AddRowInTable(DataTableC.Rows[positionC - 1], DataTable);
                                        counterC--;
                                        pickedC = false;
                                        await Task.Delay(2010 - Slider);
                                    }
                                }
                                else
                                {
                                    Steps.Add($"{tempA} < {tempB} \n" +
                                    $"Добавляем {tempA} в таблицу\n");
                                    ColorTable(DataTableA, positionA - 1, seriaA);
                                    AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                                    counterA--;
                                    pickedA = false;
                                    await Task.Delay(2010 - Slider);
                                }
                            }
                            else
                            {
                                if (pickedC)
                                {
                                    string tempC = string.Format("{0}", DataTableC.Rows[positionC - 1][_columnNumber]);
                                    if (CompareDifferentTypes(tempB, tempC))
                                    {
                                        Steps.Add($"{tempB} < {tempC} \n" +
                                    $"Добавляем {tempB} в таблицу\n");
                                        ColorTable(DataTableB, positionB - 1, seriaB);
                                        AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                                        counterB--;
                                        pickedB = false;
                                        await Task.Delay(2010 - Slider);
                                    }
                                    else
                                    {
                                        Steps.Add($"{tempB} > {tempC} \n" +
                                    $"Добавляем {tempC} в таблицу\n");
                                        ColorTable(DataTableC, positionC - 1, seriaC);
                                        AddRowInTable(DataTableC.Rows[positionC - 1], DataTable);
                                        counterC--;
                                        pickedC = false;
                                        await Task.Delay(2010 - Slider);
                                    }
                                }
                                else
                                {
                                    Steps.Add($"{tempA} > {tempB} \n" +
                                    $"Добавляем {tempB} в таблицу\n");
                                    ColorTable(DataTableB, positionB - 1, seriaB);
                                    AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                                    counterB--;
                                    pickedB = false;
                                    await Task.Delay(2010 - Slider);
                                }
                            }
                        }
                        else if (pickedC)
                        {
                            string tempA = string.Format("{0}", DataTableA.Rows[positionA - 1][_columnNumber]);
                            string tempC = string.Format("{0}", DataTableC.Rows[positionC - 1][_columnNumber]);
                            if (CompareDifferentTypes(tempA, tempC))
                            {
                                Steps.Add($"{tempA} < {tempC} \n" +
                                    $"Добавляем {tempA} в таблицу\n");
                                ColorTable(DataTableA, positionA - 1, seriaA);
                                AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                                counterA--;
                                pickedA = false;
                                await Task.Delay(2010 - Slider);
                            }
                            else
                            {
                                Steps.Add($"{tempA} > {tempC} \n" +
                                    $"Добавляем {tempC} в таблицу\n");
                                ColorTable(DataTableC, positionC - 1, seriaC);
                                AddRowInTable(DataTableC.Rows[positionC - 1], DataTable);
                                counterC--;
                                pickedC = false;
                                await Task.Delay(2010 - Slider);
                            }
                        }
                        else
                        {
                            Steps.Add($"Добавляем оставшуюся строку из А в таблицу\n");
                            ColorTable(DataTableA, positionA - 1, seriaA);
                            AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                            counterA--;
                            pickedA = false;
                            await Task.Delay(2010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        if (pickedC)
                        {
                            string tempB = string.Format("{0}", DataTableB.Rows[positionB - 1][_columnNumber]);
                            string tempC = string.Format("{0}", DataTableC.Rows[positionC - 1][_columnNumber]);
                            if (CompareDifferentTypes(tempB, tempC))
                            {
                                Steps.Add($"{tempB} < {tempC} \n" +
                                    $"Добавляем {tempB} в таблицу\n");
                                ColorTable(DataTableB, positionB - 1, seriaB);
                                AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                                counterB--;
                                pickedB = false;
                                await Task.Delay(2010 - Slider);
                            }
                            else
                            {
                                Steps.Add($"{tempB} > {tempC} \n" +
                                    $"Добавляем {tempC} в таблицу\n");
                                ColorTable(DataTableC, positionC - 1, seriaC);
                                AddRowInTable(DataTableC.Rows[positionC - 1], DataTable);
                                counterC--;
                                pickedC = false;
                                await Task.Delay(2010 - Slider);
                            }
                        }
                        else
                        {
                            Steps.Add($"Добавляем оставшуюся строку из В в таблицу\n");
                            ColorTable(DataTableB, positionB - 1, seriaB);
                            AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                            counterB--;
                            pickedB = false;
                            await Task.Delay(2010 - Slider);
                        }
                    }
                    else if (pickedC)
                    {
                        Steps.Add($"Добавляем оставшуюся строку из С в таблицу\n");
                        ColorTable(DataTableC, positionC - 1, seriaC);
                        AddRowInTable(DataTableC.Rows[positionC - 1], DataTable);
                        counterC--;
                        pickedC = false;
                        await Task.Delay(2010 - Slider);
                    }
                }
                Steps.Add($"Закончили слияние А, В и С\n" +
                    $"Увеличиваем размер серии: {_iterations} * 2 = {_iterations * 2}\n");
                _iterations *= 3; // увеличиваем размер серии в 3 раза
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
                DataTableC.Rows.Clear();
            }
            ChangeMainTable();
            TableReader.SaveChangesToCsv(_table, _folderPath);
        }



        //Natural Merge
        private void DoNatureMerge()
        {
            Steps.Clear();
            _iterations = 1;
            _segments = 1;
            DataTable dataTableA = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableA.Columns.Add(column.Name);
            }
            DataTableA.Rows.Clear();
            DataTableA = dataTableA;
            DataGridA.Columns[DataGridA.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DataTable dataTableB = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableB.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTableB;
            DataGridB.Columns[DataGridB.Columns.Count - 1].Visibility = System.Windows.Visibility.Collapsed;
            DoNatureSort();
        }
        private List<int> _seriesA = new List<int>();
        private List<int> _seriesB = new List<int>();
        async void DoNatureSort()
        {
            while (true)
            {
                _segments = 1;
                DataRow prev = DataTable.Rows[0];
                bool flag = true; bool isFirst = true;
                int seriaCounter = 0;
                string seria = "0";
                AddRowInTable(prev, DataTableA);
                int counter = 0;
                Steps.Add($"Разделяем исходную таблицу на две таблицы,\n" +
                   $"ища отсортированные серии\n");
                Steps.Add($"Записываем первую строку в таблицу А");
                foreach (DataRow cur in DataTable.Rows)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    string tempA = string.Format("{0}", prev[_columnNumber]);
                    string tempB = string.Format("{0}", cur[_columnNumber]);
                    if (CompareDifferentTypes(tempB, tempA))
                    {
                        flag = !flag;
                        _segments++;
                        counter = 0;
                        seriaCounter++;
                        if (seriaCounter == 2)
                        {
                            seria = (seria == "0") ? "1" : "0";
                            seriaCounter = 0;
                        }
                    }
                    if (flag)
                    {
                        Steps.Add($"Записываем строку с {tempB} в таблицу А");
                        AddRowInTable(cur, DataTableA, seria);
                        await Task.Delay(2010 - Slider);
                        counter++;
                    }
                    else
                    {
                        Steps.Add($"Записываем строку с {tempB} в таблицу В");
                        AddRowInTable(cur, DataTableB, seria);
                        await Task.Delay(2010 - Slider);
                        counter++;
                    }
                    prev = cur;
                }

                if (_segments == 1)
                {
                    Steps.Add($"Все элементы оказались в одной таблице\n" +
                        $"Таблица отсортирована по полю {SelectedColumn}");
                    break;
                }

                DataTable.Rows.Clear();
                DataRow newRowA = DataTable.NewRow();
                DataRow newRowB = DataTable.NewRow();

                SelectNewSeries(DataTableA, _seriesA);
                SelectNewSeries(DataTableB, _seriesB);

                bool pickedA = false, pickedB = false;
                int positionA = 0, positionB = 0;
                if(_seriesB.Count > 0) { }
                int seriaA = _seriesA[0]; int seriaB = _seriesB[0];
                string strSeriaA = "0";
                string strSeriaB = "0";
                int indA = 0; int indB = 0;
                bool endA = false; bool endB = false;
                Steps.Add($"Начинаем слияние таблиц А и В\n");
                while (true)
                {
                    if (endA && endB)
                    {
                        break;
                    }
                    if (seriaA == 0 && seriaB == 0)
                    {
                        indA++; indB++;
                        if (indA <= _seriesA.Count - 1)
                        {
                            seriaA = _seriesA[indA];
                            Steps.Add($"Следующая серия в таблице A: {seriaA} элементов");
                        }
                        if(indB <= _seriesB.Count - 1)
                        {
                            seriaB = _seriesB[indB];
                            Steps.Add($"Следующая серия в таблице В: {seriaB} элементов");
                        }
                    }
                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (seriaA > 0)
                        {
                            if (!pickedA)
                            {
                                strSeriaA = string.Format("{0}", DataTableA.Rows[positionA]["Seria"]);
                                ColorTable(DataTableA, positionA, "2");
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
                        if (seriaB > 0)
                        {
                            if (!pickedB)
                            {
                                strSeriaB = string.Format("{0}", DataTableB.Rows[positionB]["Seria"]);
                                ColorTable(DataTableB, positionB, "2");
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
                    await Task.Delay(2010 - Slider);

                    string tempA = string.Format("{0}", DataTableA.Rows[positionA - 1][_columnNumber]);
                    string tempB = string.Format("{0}", DataTableB.Rows[positionB - 1][_columnNumber]);
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                Steps.Add($"{tempA} < {tempB} \n" +
                                    $"Добавляем {tempA} в таблицу");
                                ColorTable(DataTableA, positionA - 1, strSeriaA);
                                AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                                pickedA = false;
                                seriaA--;
                                await Task.Delay(2010 - Slider);
                            }
                            else
                            {
                                Steps.Add($"{tempA} > {tempB} \n" +
                                    $"Добавляем {tempB} в таблицу");
                                ColorTable(DataTableB, positionB - 1, strSeriaB);
                                AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                                pickedB = false;
                                seriaB--;
                                await Task.Delay(2010 - Slider);
                            }
                        }
                        else
                        {
                            Steps.Add($"Добавляем {tempA} из А в таблицу");
                            ColorTable(DataTableA, positionA - 1, strSeriaA);
                            AddRowInTable(DataTableA.Rows[positionA - 1], DataTable);
                            pickedA = false;
                            seriaA--;
                            await Task.Delay(2010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        Steps.Add($"Добавляем {tempB} из В в таблицу");
                        ColorTable(DataTableB, positionB - 1, strSeriaB);
                        AddRowInTable(DataTableB.Rows[positionB - 1], DataTable);
                        pickedB = false;
                        seriaB--;
                        await Task.Delay(2010 - Slider);
                    }
                }
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
                Steps.Add($"Закончили слияние А и В\n");
            }
            ChangeMainTable();
            TableReader.SaveChangesToCsv(_table, _folderPath);
        }

        private void SelectNewSeries(DataTable dataTable, List<int> series)
        {
            series.Clear();
            string seria = "0";
            string prev = dataTable.Rows[0][_columnNumber].ToString();
            string cur;
            DataTable temp = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                temp.Columns.Add(column.Name);
            }
            AddRowInTable(dataTable.Rows[0], temp, seria);
            int count = 0;
            for (int i = 1; i < dataTable.Rows.Count; i++)
            {
                cur = dataTable.Rows[i][_columnNumber].ToString();
                if (CompareDifferentTypes(cur, prev))
                {
                    seria = (seria == "0") ? "1" : "0";
                    AddRowInTable(dataTable.Rows[i], temp, seria);
                    series.Add(count + 1);
                    count = 0;
                    prev = cur;
                    if (i == dataTable.Rows.Count - 1)
                    {
                        series.Add(count + 1);
                        seria = (seria == "0") ? "1" : "0";
                    }
                    continue;
                }
                count++;
                AddRowInTable(dataTable.Rows[i], temp, seria);
                if (i == dataTable.Rows.Count - 1)
                {
                    series.Add(count + 1);
                    seria = (seria == "0") ? "1" : "0";
                }
                prev = cur;
            }
            if(dataTable.Rows.Count == 0) 
            {
                series.Add(1);
            }
            dataTable.Rows.Clear();
            foreach(DataRow row in temp.Rows)
            {
                dataTable.Rows.Add(row.ItemArray);
            }
        }
        private bool Check()
        {
            if (_folderPath == "")
            {
                MessageBox.Show("Выберите файл с таблицей");
                return false;
            }
            if(SelectedSort == null || SelectedSort == "" || SelectedColumn == null || SelectedColumn == "")
            {
                MessageBox.Show("Выберите поле и алгоритм сортировки");
                return false;
            }
            _columnNumber = TableReader.GetColumnNumber(SelectedColumn, _scheme);
            return true;
        }
    }
}
