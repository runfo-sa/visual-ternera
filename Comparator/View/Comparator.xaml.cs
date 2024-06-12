using Comparator.Model;
using Comparator.ViewModel;
using Core.Interfaces;
using DiffPlex.DiffBuilder.Model;
using System.Windows;
using System.Windows.Controls;

namespace Comparator
{
    public partial class Comparator : UserControl, IContent
    {
        public static string Title => "Comparar Etiquetas";

        public Comparator()
        {
            InitializeComponent();
            DataContextChanged += LoadViewModel;
            leftEditor.Options.AllowScrollBelowDocument = true;
            rightEditor.Options.AllowScrollBelowDocument = true;
        }

        private void LoadViewModel(Object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = (ComparatorViewModel)DataContext;
            vm.CalculateDiffEvent += () =>
            {
                Loaded += (o, e) => RenderDiff(vm.Diff);
            };
            vm.OpenSelector();
        }

        private void LeftEditor_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            rightEditor.ScrollToVerticalOffset(leftEditor.VerticalOffset);
        }

        private void RightEditor_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            leftEditor.ScrollToVerticalOffset(rightEditor.VerticalOffset);
        }

        private void RenderDiff(SideBySideDiffModel diff)
        {
            leftEditor.TextArea.TextView.BackgroundRenderers.Clear();
            rightEditor.TextArea.TextView.BackgroundRenderers.Clear();

            leftEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(diff.OldText.Lines));
            rightEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(diff.NewText.Lines));
        }
    }
}
