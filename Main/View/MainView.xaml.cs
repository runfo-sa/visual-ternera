using System.Diagnostics;
using System.Windows;
using Wpf.Ui.Appearance;

namespace Main
{
    public partial class MainView : Window
    {
        public MainView()
        {
            SystemThemeWatcher.Watch(this);
            ApplicationThemeManager.Apply(ApplicationTheme.Dark);
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(Object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }
    }
}
