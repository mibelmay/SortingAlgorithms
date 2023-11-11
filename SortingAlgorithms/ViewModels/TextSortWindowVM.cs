using SortingAlgorithms.Models;
using System;
using System.Collections.Generic;
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

        public ICommand Sort => new CommandDelegate(param =>
        {
            SelectSort();
        });
        private void SelectSort()
        {
            AlgorithmProfiler profiler = new AlgorithmProfiler();
            string[] words = _initialText.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            switch (SortName)
            {
                case "Insertion Sort":
                    InsertionSort insertionSort = new InsertionSort();
                    Result result = profiler.RunExtra(words, insertionSort);
                    ShowSorted(result.Sorted);
                    break;
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
    }
}
