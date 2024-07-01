using AdonisUI.Controls;

namespace Main.Views
{
    public partial class ExceptionPopUp : AdonisWindow
    {
        public ExceptionPopUp(string exceptionMessage)
        {
            InitializeComponent();
            errorMessage.Text = exceptionMessage;
        }

        private void Confirm(Object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
