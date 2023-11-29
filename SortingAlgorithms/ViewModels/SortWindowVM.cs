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
using System.Runtime.CompilerServices;

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
        private string _sortName = "ShellSort";
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
        private Canvas _canvasMerged = new Canvas();
        public Canvas CanvasMerged 
        {
            get { return _canvasMerged; } set { _canvasMerged = value; OnPropertyChanged(); } }
        private int _slider = 1000;
        public int Slider
        {
            get { return _slider; }
            set { _slider = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> Comments { get; set; } = new ObservableCollection<string>();

        public List<Element> Array { get; set; } // максимум/минимум 100/-100
        private Movement _movement = null;
        private MergedMovement _mergedMovement = null;
        private bool _isMerged = false;
        public bool IsMerged
        {
            get { return _isMerged; }
            set { _isMerged = value; OnPropertyChanged(); }
        }
        //для отрисовки
        private double columnHeight;


        public ICommand Start => new CommandDelegate(param =>
        {
            Comments.Clear();
            Canvas.Children.Clear();
            CanvasMerged.Children.Clear();
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
            Canvas.Children.Clear();
            CanvasMerged.Children.Clear();
            Array = Parser.GenerateVector();
            GetReady(Array);
        });

        public void GetReady(List<Element> array)
        {
            Canvas.Children.Clear();
            int columnWidth = 15;
            int max = array.Max(x => x.Data);
            int min = array.Min(x => x.Data);
            int height = (SortName == "Merge Sort") ? 200 : 400;
            columnHeight = Math.Abs(max) > Math.Abs(min) ? height / Math.Abs(max) : height / Math.Abs(min);
            double xPosition = (845 - (array.Count + 2) * columnWidth) / 2;

            foreach (Element item in array)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.Data.ToString();
                textBlock.FontSize = 10;
                textBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff");
                
                Rectangle column = new Rectangle();
                column.Width = columnWidth;
                column.Height = columnHeight * Math.Abs(item.Data);
               
                Canvas.SetLeft(textBlock, xPosition);
                Canvas.SetBottom(textBlock, -224 + item.Data * columnHeight);
                Canvas.Children.Add(textBlock);
                Canvas.SetBottom(column, -224);
               
                Canvas.SetLeft(column, xPosition);
                xPosition += columnWidth + 2;
                int i;
                if(_movement != null && (_movement.IdFrom == item.Id || _movement.IdTo == item.Id))
                {
                    column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#f6bd60");
                }
                else if (_movement != null && (SortName == "Merge Sort" || SortName == "Heap Sort") && (i = CheckMerge(_movement.MergedArrays, item)) > 0)
                {
                    if (i == 1) { column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#e26d5c"); }
                    else { column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#723d46"); }
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
            if (_movement != null && _movement.Comment != "")
            {
                Comments.Add(_movement.Comment);
            }
        }

        private void GetReadyMerge(List<Element> array)
        {
            if(array == null || array.Count == 0) { return; }
            CanvasMerged.Children.Clear();
            int columnWidth = 15;
            
            double xPosition = (845 - (array.Count + 2) * columnWidth) / 2;
            foreach (Element item in array)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.Data.ToString();
                textBlock.FontSize = 10;
                textBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff");

                Rectangle column = new Rectangle();
                column.Width = columnWidth;
                column.Height = columnHeight * Math.Abs(item.Data);

                Canvas.SetLeft(textBlock, xPosition);
                Canvas.SetBottom(textBlock, -225 + item.Data * columnHeight);
                CanvasMerged.Children.Add(textBlock);
                Canvas.SetBottom(column, -225);

                Canvas.SetLeft(column, xPosition);
                xPosition += columnWidth + 2;
                int i;
                if (_mergedMovement != null && _mergedMovement.Id == item.Id)
                {
                    column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#f6bd60");
                }
                else
                {
                    column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#a3b18a");
                }
                column.RadiusX = 2;
                column.RadiusY = 2;
                column.StrokeThickness = 1;
                CanvasMerged.Children.Add(column);
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
                if (tuple.Item2 == null || i >= tuple.Item2.Length) { continue; }
                if (tuple.Item2[i].Id == item.Id) 
                {
                    return 2;
                }
            }
            if (tuple.Item1.Length == 0)
            {
                for (int i = 0; i < tuple.Item2.Length; i++)
                {
                    if (tuple.Item2[i].Id == item.Id)
                    {
                        return 2;
                    }
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
                    AnimateMerged(mergedsort.Movements, mergedsort.NewMovements);
                    break;
                default:
                    break;
            }
        }
        public async void AnimateMerged(List<Movement> movements, List<MergedMovement> newArray)
        {
            for (int i = 0; i < movements.Count; i++)
            {
                _movement= movements[i];
                _mergedMovement = newArray[i];
                GetReady(movements[i].Elements);
                GetReadyMerge(newArray[i].Elements);
                await Task.Delay(2010 - Slider);
            }
            Comments.Add("Массив отсортирован!");
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
            Canvas.Children.Clear();
            CanvasMerged.Children.Clear();
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
