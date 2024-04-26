using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Main.Model
{
    [ValueConversion(typeof(Status), typeof(Brush))]
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (Status)value;
            return status switch
            {
                Status.DesactualizadaSobrantes => Brushes.Firebrick,
                Status.MultipleInstalaciones => Brushes.OrangeRed,
                Status.Desactualizada => Brushes.Tomato,
                Status.NoInstalado => Brushes.DarkRed,
                Status.Sobrantes => Brushes.Orange,
                Status.Okay => Brushes.LightSeaGreen,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}