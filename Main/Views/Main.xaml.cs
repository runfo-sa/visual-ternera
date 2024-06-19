using AdonisUI.Controls;
using System.Diagnostics;
using System.Windows;

namespace Main.Views
{
    public partial class Main : AdonisWindow
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(Object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            }
            catch (Exception)
            {
                ((App)Application.Current).ResolveException(null,
                    new UnhandledExceptionEventArgs(
                        new Exception("Problemas para abrir el enlace, asegurarse de configurar el link al repositorio."),
                        false));
            }
        }
    }
}
