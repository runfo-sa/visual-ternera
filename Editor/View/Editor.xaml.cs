using Core;
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
    }
}