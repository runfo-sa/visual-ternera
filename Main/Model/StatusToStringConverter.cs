using System.Globalization;
using System.Windows.Data;

namespace Main.Model
{
    [ValueConversion(typeof(Status), typeof(string))]
    public class StatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (Status)value;
            return status switch
            {
                Status.DesactualizadaSobrantes => "Desactualizado y Sobrantes",
                Status.MultipleInstalaciones => "Multiples Instalaciones",
                Status.Desactualizada => "Desactualizado",
                Status.NoInstalado => "PiQuatro No Instalado",
                Status.Sobrantes => "Archivos Sobrantes",
                Status.Okay => "Okay",
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}