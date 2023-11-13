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

namespace SortingAlgorithms.ViewModels
{
    class OuterSortWindowVM : ViewModel
    {
        public ObservableCollection<string> Steps { get; set; } = new ObservableCollection<string>();
        private string _folderPath = "";
        private TableScheme _scheme;
        private Table _table;
        public List<string> SortNames { get; set; } = new List<string>() { "Прямое слияние" , "Трехпутевое слияние"};
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
        private string _delay;
        public string Delay
        {
            get { return _delay; }
            set { _delay = value; OnPropertyChanged(); }
        }
        private int _delayInSeconds;

        //для сортировки
        private int _iterations = 1;
        private int _segments;
        private int _columnNumber;

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
                    }
                    if (flag)
                    {
                        Steps.Add($"Добавляем строку {counter1} в таблицу А\n");
                        AddRowInTable(row, DataTableA);
                        counter++;
                        await Task.Delay(_delayInSeconds);
                    }
                    else
                    {
                        Steps.Add($"Добавляем строку {counter1} в таблицу В\n");
                        AddRowInTable(row, DataTableB);
                        counter++;
                        await Task.Delay(_delayInSeconds);
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
                DataRow newRowA = DataTable.NewRow();
                DataRow newRowB = DataTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                bool pickedA = false, pickedB = false, endA = false, endB = false;
                int positionA = 0;
                int positionB = 0;
                int currentPA = 0;
                int currentPB = 0;
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
                            string tempA = string.Format("{0}", newRowA[_columnNumber]);
                            string tempB = string.Format("{0}", newRowB[_columnNumber]);
                            Steps.Add($"Сравниваем {tempA} и {tempB}");
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                Steps.Add($"{tempA} < {tempB} \n" +
                                    $"Добавляем {tempA} в таблицу\n");
                                AddRowInTable(newRowA, DataTable);
                                counterA--;
                                pickedA = false;

                                await Task.Delay(_delayInSeconds);
                            }
                            else
                            {
                                Steps.Add($"{tempA} > {tempB} \n" +
                                    $"Добавляем {tempB} в таблицу\n");
                                AddRowInTable(newRowB, DataTable);
                                counterB--;
                                pickedB = false;
                            }
                        }
                        else
                        {
                            Steps.Add($"Добавляем оставшуюся строку из А в таблицу\n");
                            AddRowInTable(newRowA, DataTable);
                            counterA--;
                            pickedA = false;

                            await Task.Delay(_delayInSeconds);
                        }
                    }
                    else if (pickedB)
                    {
                        Steps.Add($"Добавляем оставшуюся строку из В в таблицу\n");
                        AddRowInTable(newRowB, DataTable);
                        counterB--;
                        pickedB = false;

                        await Task.Delay(_delayInSeconds);
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

        private void DoThreeWayMerge() //добавляет колонки во вспомогательные таблицы
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
            DataTable dataTableC = new DataTable();
            foreach (Column column in _scheme.Columns)
            {
                dataTableC.Columns.Add(column.Name);
            }
            DataTableC.Rows.Clear();
            DataTableC = dataTableC;
            DoThreeWaySort();
        }

        //Three-way merge
        async void DoThreeWaySort()
        {
            while (true)
            {
                _segments = 1;
                bool flag = true;
                DataRow prev = DataTable.Rows[0];
                AddRowInTable(prev, DataTableA);
                foreach (DataRow row in DataTable.Rows)
                {
                    if(flag)
                    {
                        flag = false;
                        continue;
                    }
                    string tempA = string.Format("{0}", prev[_columnNumber]);
                    string tempB = string.Format("{0}", row[_columnNumber]);
                    if(CompareDifferentTypes(tempB, tempA))
                    {
                        _segments++;
                    }
                    if (_segments % 3 == 1)
                    {
                        AddRowInTable(row, DataTableA);
                        await Task.Delay(_delayInSeconds);
                    }
                    else if (_segments % 3 == 2)
                    {
                       AddRowInTable(row, DataTableB);
                       await Task.Delay(_delayInSeconds);
                    }
                    else
                    {
                        AddRowInTable(row, DataTableC);
                        await Task.Delay(_delayInSeconds);
                    }
                    prev = row;
                }

                if (_segments == 1)
                {
                    break;
                }
                DataTable.Rows.Clear();
                DataRow newRowA = DataTable.NewRow();
                DataRow newRowB = DataTable.NewRow();
                DataRow newRowC = DataTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                int counterC = _iterations;
                bool pickedA = false, pickedB = false, pickedC = false, endA = false, endB = false, endC = false;
                int positionA = 0; int positionB = 0; int positionC = 0;
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

                    if (positionC != DataTableC.Rows.Count)
                    {
                        if (counterC > 0)
                        {
                            if (!pickedC)
                            {
                                newRowC = DataTableC.Rows[positionC];
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

                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            string tempA = string.Format("{0}", newRowA[_columnNumber]);
                            string tempB = string.Format("{0}", newRowB[_columnNumber]);
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                if (pickedC)
                                {
                                    string tempC = string.Format("{0}", newRowC[_columnNumber]);
                                    if (CompareDifferentTypes(tempA, tempC))
                                    {
                                        AddRowInTable(newRowA, DataTable);
                                        counterA--;
                                        pickedA = false;
                                        await Task.Delay(_delayInSeconds);
                                    }
                                    else
                                    {
                                        AddRowInTable(newRowC, DataTable);
                                        counterC--;
                                        pickedC = false;
                                        await Task.Delay(_delayInSeconds);
                                    }
                                }
                                else
                                {
                                    AddRowInTable(newRowA, DataTable);
                                    counterA--;
                                    pickedA = false;
                                    await Task.Delay(_delayInSeconds);
                                }
                            }
                            else
                            {
                                if (pickedC)
                                {
                                    string tempC = string.Format("{0}", newRowC[_columnNumber]);
                                    if (CompareDifferentTypes(tempB, tempC))
                                    {
                                        AddRowInTable(newRowB, DataTable);
                                        counterB--;
                                        pickedB = false;
                                        await Task.Delay(_delayInSeconds);
                                    }
                                    else
                                    {
                                        AddRowInTable(newRowC, DataTable);
                                        counterC--;
                                        pickedC = false;
                                        await Task.Delay(_delayInSeconds);
                                    }
                                }
                                else
                                {
                                    AddRowInTable(newRowB, DataTable);
                                    counterB--;
                                    pickedB = false;
                                    await Task.Delay(_delayInSeconds);
                                }
                            }
                        }
                        else if (pickedC)
                        {
                            string tempA = string.Format("{0}", newRowA[_columnNumber]);
                            string tempC = string.Format("{0}", newRowC[_columnNumber]);
                            if (CompareDifferentTypes(tempA, tempC))
                            {
                                AddRowInTable(newRowA, DataTable);
                                counterA--;
                                pickedA = false;
                                await Task.Delay(_delayInSeconds);
                            }
                            else
                            {
                                AddRowInTable(newRowC, DataTable);
                                counterC--;
                                pickedC = false;
                                await Task.Delay(_delayInSeconds);
                            }
                        }
                        else
                        {
                            AddRowInTable(newRowA, DataTable);
                            counterA--;
                            pickedA = false;
                            await Task.Delay(_delayInSeconds);
                        }
                    }
                    else if (pickedB)
                    {
                        if (pickedC)
                        {
                            string tempB = string.Format("{0}", newRowB[_columnNumber]);
                            string tempC = string.Format("{0}", newRowC[_columnNumber]);
                            if (CompareDifferentTypes(tempB, tempC))
                            {
                                AddRowInTable(newRowB, DataTable);
                                counterB--;
                                pickedB = false;
                                await Task.Delay(_delayInSeconds);
                            }
                            else
                            {
                                AddRowInTable(newRowC, DataTable);
                                counterC--;
                                pickedC = false;
                                await Task.Delay(_delayInSeconds);
                            }
                        }
                        else
                        {
                            AddRowInTable(newRowB, DataTable);
                            counterB--;
                            pickedB = false;
                            await Task.Delay(_delayInSeconds);
                        }
                    }
                    else if (pickedC)
                    {
                        AddRowInTable(newRowC, DataTable);
                        counterC--;
                        pickedC = false;
                        await Task.Delay(_delayInSeconds);
                    }
                }
                _iterations *= 2; // увеличиваем размер серии в 2 раза
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
                DataTableC.Rows.Clear();
            }
            ChangeMainTable();
            TableReader.SaveChangesToCsv(_table, _folderPath);
        }
        private bool Check()
        {
            if (_folderPath == "")
            {
                MessageBox.Show("Выберите файл с таблицей");
                return false;
            }
            if (!double.TryParse(_delay, out double millis))
            {
                Delay = "Incorrect";
                return false;
            }
            _delayInSeconds = (int) (millis * 1000);
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
