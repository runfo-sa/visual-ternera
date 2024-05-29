using AdonisUI.Controls;
using Core;
using System.Collections.ObjectModel;

namespace Cohere.View
{
    /// <summary>
    /// Interaction logic for SelectRuleDialog.xaml
    /// </summary>
    public partial class SelectRuleDialog : AdonisWindow
    {
        public ObservableCollection<Regla> Reglas { get; set; }

        public Regla SelectedRule
        {
            get => (Regla)dataGrid.SelectedItem;
        }

        public SelectRuleDialog(Settings settings)
        {
            InitializeComponent();
            DataContext = this;
            using (var context = new IdeDbContext(settings.SqlConnection))
            {
                Reglas = [.. context.Reglas];
            }
            dataGrid.ItemsSource = Reglas;
        }

        private void dataGrid_SelectionChanged(Object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            aceptarButton.IsEnabled = true;
        }

        private void aceptarButton_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
