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
            vm.GetReady(vm.Array);
            window.ShowDialog();
        });
    }
}
