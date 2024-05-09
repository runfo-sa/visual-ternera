using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Main.Model
{
    [ValueConversion(typeof(DateTime), typeof(Brush))]
    public class DateTimeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            if (dateTime < DateTime.Now.AddHours(-4))
                return "#815e3b";

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
