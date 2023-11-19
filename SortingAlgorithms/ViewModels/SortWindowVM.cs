using System;
using System.Collections.Generic;
using System.Configuration;
using SortingAlgorithms.Models;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace SortingAlgorithms.ViewModels
{
    public class SortWindowVM : ViewModel
    {
        private List<string> _sortNames = new List<string>() { "Shell Sort" , "Heap Sort", "Insertion Sort", "Merge Sort" };
        public List<string> SortNames
        {
            get { return _sortNames; }
            set
            {
                _sortNames = value;
                OnPropertyChanged();
            }
        }
        private string _sortName;
        public string SortName
        {
            get { return _sortName; }
            set
            {
                _sortName = value;
                OnPropertyChanged();
            }
        }
        private Canvas _canvas = new Canvas();
        public Canvas Canvas
        {
            get { return _canvas; }
            set
            {
                _canvas = value;
                OnPropertyChanged();
            }
        }
        private int _slider = 1000;
        public int Slider
        {
            get { return _slider; }
            set { _slider = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> Comments { get; set; } = new ObservableCollection<string>();

        public List<Element> Array { get; set; } // максимум/минимум 100/-100
        private Movement _movement = null;


        public ICommand Start => new CommandDelegate(param =>
        {
            Comments.Clear();
            //старт отрисовки
            if (!Check())
            {
                return;
            }
            GetReady(Array);
            SelectSort();
        });

        public ICommand GenerateArray => new CommandDelegate(param =>
        {
            Comments.Clear();
            Array = Parser.GenerateVector();
            GetReady(Array);
        });

        public void GetReady(List<Element> array)
        {
            Canvas.Children.Clear();
            int columnWidth = 15;
            int max = array.Max(x => x.Data);
            int min = array.Min(x => x.Data);
            double columnHeight = Math.Abs(max) > Math.Abs(min) ? 200 / Math.Abs(max) : 200 / Math.Abs(min);
            double xPosition = (870 - (array.Count + 2) * columnWidth) / 2;

            foreach (Element item in array)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.Data.ToString();
                textBlock.FontSize = 10;
                textBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff");
                
                Rectangle column = new Rectangle();
                column.Width = columnWidth;
                column.Height = columnHeight * Math.Abs(item.Data);
                if (item.Data < 0)
                {
                    Canvas.SetLeft(textBlock, xPosition);
                    Canvas.SetBottom(textBlock, -215 + item.Data * columnHeight - 15);
                    Canvas.Children.Add(textBlock);
                    Canvas.SetBottom(column, -215 + item.Data * columnHeight);
                }
                else
                {
                    Canvas.SetLeft(textBlock, xPosition);
                    Canvas.SetBottom(textBlock, -215 + item.Data * columnHeight);
                    Canvas.Children.Add(textBlock);
                    Canvas.SetBottom(column, -215);
                }
                Canvas.SetLeft(column, xPosition);
                xPosition += columnWidth + 2;
                int i;
                if(_movement != null && (_movement.IdFrom == item.Id || _movement.IdTo == item.Id))
                {
                    column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#f6bd60");
                }
                else if (_movement != null && SortName == "Merge Sort" && (i = CheckMerge(_movement.MergedArrays, item)) > 0)
                {
                    column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#e26d5c");
                    //column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#723d46");
                }
                else
                {
                    column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#a3b18a");
                }
                column.RadiusX = 2;
                column.RadiusY = 2;
                column.StrokeThickness = 1;
                Canvas.Children.Add(column);
            }
            if (_movement != null)
            {
                Comments.Add(_movement.Comment);
            }
        }

        private int CheckMerge(Tuple<Element[], Element[]> tuple, Element item)
        {
            if (tuple == null) { return 0; }

            for (int i = 0; i < tuple.Item1.Length; i++)
            {
                if (tuple.Item1[i].Id == item.Id)
                {
                    return 1;
                }
                if (i >= tuple.Item2.Length) { continue; }
                if (tuple.Item2[i].Id == item.Id) 
                {
                    return 2;
                }
            }
            return 0;
        }
        public void SelectSort()
        {
            switch (SortName)
            {
                case "Shell Sort":
                    ShellSort shellSort = new ShellSort();
                    shellSort.Execute(Element.CopyElements(Array));
                    Animate(shellSort.Movements);
                    break;
                case "Heap Sort":
                    HeapSort heapsort = new HeapSort();
                    heapsort.Execute(Element.CopyElements(Array));
                    Animate(heapsort.Movements);
                    break;
                case "Insertion Sort":
                    InsertionSort insertionsort = new InsertionSort();
                    insertionsort.Execute(Element.CopyElements(Array));
                    Animate(insertionsort.Movements);
                    break;
                case "Merge Sort":
                    MergedSort mergedsort = new MergedSort();
                    mergedsort.Execute(Element.CopyElements(Array));
                    Animate(mergedsort.Movements);
                    break;
                default:
                    break;
            }
        }

        public async void Animate(List<Movement> movements)
        {
            foreach (Movement movement in movements)
            {
                _movement = movement;
                GetReady(movement.Elements);
                await Task.Delay(2010 - Slider);
            }
            Comments.Add("Массив отсортирован!");
        }

        public bool Check()
        {
            if (SortName == null || SortName == "")
            {
                MessageBox.Show("Выберите сортировку");
                return false;
            }
            return true;
        }
        public ICommand LoadArray => new CommandDelegate(param => 
        {
            Comments.Clear();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string path = "";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                Array = Parser.LoadFromFile(path);
                GetReady(Array);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        });
    }
}
