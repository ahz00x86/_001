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
    public class RoomToBackgroudColor : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            if (value is Room)
            {
                Room room = (Room)value;

                Int16 i;
                Int16.TryParse((String)parameter, out i);

                if (room.MyIdUsuario == room.Users[i])
                {
                    return "Red";
                }

                return room.UsersOcupation[i] ? "DodgerBlue" : "Green";
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
