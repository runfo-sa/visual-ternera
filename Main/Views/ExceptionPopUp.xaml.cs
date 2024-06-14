using AdonisUI.Controls;
using Core.Logger;

namespace Main.View
{
    public partial class ExceptionPopUp : AdonisWindow
    {
        public ExceptionPopUp(string exceptionMessage)
        {
            InitializeComponent();
            errorMessage.Text = exceptionMessage;
        }

        private void okButton_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Log(errorMessage.Text);
            DialogResult = true;
        }
    }
}
