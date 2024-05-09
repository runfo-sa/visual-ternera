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
                Status.DesactualizadaSobrantes => "#DD5746",
                Status.MultipleInstalaciones => "#8B322C",
                Status.Desactualizada => "#DD5746",
                Status.NoInstalado => "#8B322C",
                Status.Sobrantes => "#FFC470",
                Status.Okay => "#4793AF",
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
