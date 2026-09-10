using FarmsWar.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FarmsWar.Converters
{
    public class DIctionaryInt16BooleanToString : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {   
            if(value is Room)
            {
                Room room = (Room)value;

                Int16 i;
                Int16.TryParse((String)parameter, out i);

                if (room.MyIdUsuario == room.Users[i])
                {
                    return "Salir";
                }

                return room.UsersOcupation[i] ? "Ocupado" : "Libre";
            }
            else
            {
                return "";
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
