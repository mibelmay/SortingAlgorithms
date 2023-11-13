using SortingAlgorithms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SortingAlgorithms.ViewModels
{
    public class TextSortWindowVM : ViewModel
    {
        private string _initialText;
        public string InitialText
        {
            get { return _initialText; } set
            {
                _initialText = value;
                OnPropertyChanged();
            }
        }
        private string _sortedText;
        public string SortedText
        {
            get { return _sortedText; }
            set
            {
                _sortedText = value; OnPropertyChanged();
            }
        }
        public List<string> SortNames { get; set; } = new List<string>() { "Insertion Sort", "ABC sort" };
        private string _sortName;
        public string SortName
        {
            get { return _sortName; }
            set
            {
                _sortName = value; OnPropertyChanged();
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

        public ICommand Sort => new CommandDelegate(param =>
        {
            if(!Check())
            {
                MessageBox.Show("Выберите сортировку");
                return;
            }
            SelectSort();
            Result result = SelectSort();
            ShowSorted(result.Sorted);
            ShowDictionary(result.WordsOfCount);
        });
        private Result SelectSort()
        {
            AlgorithmProfiler profiler = new AlgorithmProfiler();
            Result result = new Result();
            string[] words = _initialText.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            switch (SortName)
            {
                case "Insertion Sort":
                    InsertionTextSort insertionSort = new InsertionTextSort();
                    return profiler.RunExtra(words, insertionSort);
                case "ABC sort":
                    ABCSortAlgorithm ABCSort = new ABCSortAlgorithm();
                    return profiler.RunExtra(words, ABCSort);
                default:
                    return null;
            }
        }

        private void ShowSorted(string[] text)
        {
            StringBuilder stringBuilder= new StringBuilder();
            foreach(string word in text)
            {
                stringBuilder.Append(word + " ");
            }
            SortedText = stringBuilder.ToString();
        }

        private void ShowDictionary(Dictionary<string, int> wordsOfCount)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("word"));
            dataTable.Columns.Add(new DataColumn("count"));

            foreach(var pair in wordsOfCount)
            {
                dataTable.Rows.Add(pair.Key, pair.Value);
            }
            DataTable = dataTable;
        }

        public ICommand LoadText => new CommandDelegate(param =>
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string path = "";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                string text = Parser.LoadText(path);
                InitialText = text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        });

        private bool Check()
        {
            if (SortName == null || SortName == "") 
                return false;
            return true;
        }
    }
}
