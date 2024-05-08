using Core;
using System.Windows.Controls;

namespace Comparator
{
    public partial class Comparator : UserControl, IContent
    {
        public static string Title => "Comparar Etiquetas";

        public Comparator()
        {
            InitializeComponent();
        }
    }
}
