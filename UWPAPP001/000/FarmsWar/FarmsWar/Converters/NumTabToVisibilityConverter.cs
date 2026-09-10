using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FarmsWar.Converters
{
    public class NumTabToVisibilityConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            if (value is Int16)
            {
                Int16 numero = (Int16)value;

                String posicion = (String)parameter;

                switch (numero)
                {
                    case 0:
                        return "Collapsed";
                    case 1:
                        return posicion == "11" ? "Visible" : "Collapsed";
                    case 2:
                        return posicion == "20" || posicion == "02" ? "Visible" : "Collapsed";
                    case 3:
                        return posicion == "20" || posicion == "02" || posicion == "11" ? "Visible" : "Collapsed";
                    case 4:
                        return posicion == "20" || posicion == "02" || posicion == "00" || posicion == "22" ? "Visible" : "Collapsed";
                    case 5:
                        return posicion == "20" || posicion == "02" || posicion == "11" || posicion == "00" || posicion == "22" ? "Visible" : "Collapsed";
                    case 6:
                        return posicion == "01" || posicion == "21" || posicion == "11" ? "Collapsed" : "Visible";

                }

                return "Visible";
            }
            else
            {
                return "Collapsed";
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

