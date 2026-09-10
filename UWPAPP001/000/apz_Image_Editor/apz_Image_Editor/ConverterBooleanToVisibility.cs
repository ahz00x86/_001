using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace apz_Image_Editor
{
    public class ConverterBooleanToVisibility: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is Boolean)
            {
                if ((Boolean)value)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
