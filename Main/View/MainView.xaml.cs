using AdonisUI.Controls;
using System.Diagnostics;

namespace Main
{
    public partial class MainView : AdonisWindow
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(Object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }
    }
}
