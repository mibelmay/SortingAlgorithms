using SortingAlgorithms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SortingAlgorithms.ViewModels
{
    public class MainWindowVM : ViewModel
    {
        public ICommand OpenSortWindow => new CommandDelegate(param => 
        {
            SortWindow window = new SortWindow();
            SortWindowVM vm = new SortWindowVM();
            window.DataContext = vm;
            window.ShowDialog();
        });

        public ICommand OpenTextSortWindow => new CommandDelegate(param =>
        {
            TextSortWindow window = new TextSortWindow();
            TextSortWindowVM vm = new TextSortWindowVM();
            window.DataContext = vm;
            window.ShowDialog();
        });

        public ICommand OpenOuterSortWindow => new CommandDelegate(param =>
        {
            OuterSortWindow window = new OuterSortWindow();
            OuterSortWindowVM vm = new OuterSortWindowVM();
            window.DataContext = vm;
            window.ShowDialog();
        });
    }
}
