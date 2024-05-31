using AdonisUI.Controls;
using Core.ViewLogic;
using System.IO;

namespace Comparator.View
{
    public partial class SelectLabelsDialog : AdonisWindow
    {
        public LabelFile FirstLabel
        {
            get => (LabelFile)firstLabel.SelectedItem;
        }

        public LabelFile SecondLabel
        {
            get => (LabelFile)secondLabel.SelectedItem;
        }

        public SelectLabelsDialog(string etiquetasDir)
        {
            InitializeComponent();

            var files = Directory.GetFiles(etiquetasDir, "*.e01").Select(f => new LabelFile(f));
            firstLabel.ItemsSource = files;
            secondLabel.ItemsSource = files;
        }

        private void Button_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
