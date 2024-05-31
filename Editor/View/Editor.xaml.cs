using Core.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Editor
{
    public partial class Editor : UserControl, IContent
    {
        public static string Title => "Editar Etiquetas";

        public Editor()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
