using FarmsWar.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace FarmsWar.Converters
{
    public class BooleanBlackBackgroundColor : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
         {
            if (value is Boolean && (Boolean)value)
            {
                return "Black";
            }
            else
            {
                return "White";
            }


        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
