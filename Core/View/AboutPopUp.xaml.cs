using System.Reflection;
using System.Windows;

namespace Core.View
{
    public partial class AboutPopUp : Window
    {
        public AboutPopUp()
        {
            InitializeComponent();
            verionText.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
