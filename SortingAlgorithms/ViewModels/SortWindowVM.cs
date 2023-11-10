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

namespace SortingAlgorithms.ViewModels
{
    public class SortWindowVM : ViewModel
    {
        private List<string> _sortNames = new List<string>() { "Shell Sort" };
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
        private string _delay;
        public string Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                OnPropertyChanged();
            }
        }
        private double delayInSeconds;
        public ObservableCollection<string> Comments { get; set; } = new ObservableCollection<string>();

        public List<Element> Array { get; set; } // максимум/минимум 100/-100
        private Movement _movement = null;


        public ICommand Start => new CommandDelegate(param =>
        {
            //старт отрисовки
            if (!Check())
            {
                return;
            }
            GetReady(Array);
            SelectSort();
        });

        public void GetReady(List<Element> array)
        {
            Canvas.Children.Clear();
            int columnWidth = 15;
            int max = array.Max(x => x.Data);
            int min = array.Min(x => x.Data);
            double columnHeight = Math.Abs(max) > Math.Abs(min) ? 235 / Math.Abs(max) : 235 / Math.Abs(min);
            double xPosition = (870 - (array.Count + 2) * columnWidth) / 2;

            foreach (Element item in array)
            {
                Rectangle column = new Rectangle();
                column.Width = columnWidth;
                column.Height = columnHeight * Math.Abs(item.Data);
                if (item.Data < 0)
                {
                    Canvas.SetBottom(column, -215 + item.Data * columnHeight);
                }
                else
                {
                    Canvas.SetBottom(column, -215);
                }
                Canvas.SetLeft(column, xPosition);
                xPosition += columnWidth + 2;
                column.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#9b2226");
                column.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#9b2226");
                column.StrokeThickness = 1;
                Canvas.Children.Add(column);
            }
            if (_movement != null)
            {
                Comments.Add(_movement.Comment);
            }
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
                await Task.Delay((int)(delayInSeconds * 1000));
            }
            Comments.Add("Массив отсортирован!");
        }

        public bool Check()
        {
            if (!double.TryParse(Delay, out double seconds))
            {
                Delay = "Incorrect";
                return false;
            }
            if (seconds > 5)
            {
                Delay = "Too much";
                return false;
            }
            if (SortName == null || SortName == "")
            {
                MessageBox.Show("Выберите сортировку");
                return false;
            }
            delayInSeconds = seconds;
            return true;
        }
        public ICommand LoadArray => new CommandDelegate(param => 
        {
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
