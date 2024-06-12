using Comparator.Model;
using Comparator.ViewModel;
using Core.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                Loaded += (o, e) => RenderDiff();
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

        private void RenderDiff()
        {
            leftEditor.TextArea.TextView.BackgroundRenderers.Clear();
            rightEditor.TextArea.TextView.BackgroundRenderers.Clear();

            Color leftColor = Color.FromArgb(50, 232, 155, 180);
            Color rightColor = Color.FromArgb(50, 181, 232, 155);

            var firstLines = leftEditor.Document?.Text.Split(Environment.NewLine);
            var secondLines = rightEditor.Document?.Text.Split(Environment.NewLine);

            if (firstLines == null || secondLines == null)
            {
                return;
            }

            List<int> diffLines = [];

            int i = 0, j = 0;
            for (; i < firstLines.Length && j < secondLines.Length; i++, j++)
            {
                if (firstLines[i] != secondLines[j])
                {
                    diffLines.Add(i + 1);
                }
            }

            diffLines.AddRange(Enumerable.Range(Int32.Min(i, j), Int32.Max(firstLines.Length, secondLines.Length)));
            var diffs = diffLines.Distinct().ToList();
            leftEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(diffs, leftColor));
            rightEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(diffs, rightColor));
        }
    }
}
