using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Drawing;
using System.Data;
using System.Windows.Media;

namespace SortingAlgorithms.Models
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataRowView drv = value as DataRowView;
            int length = drv.Row.ItemArray.Length;
            string status = (string)drv[length - 1];
                switch (status)
                {
                    case "0":
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#723d46");
                    case "1":
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#e26d5c");
                    case "2":
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#344e41");
                    default:
                        return DependencyProperty.UnsetValue;
                }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
