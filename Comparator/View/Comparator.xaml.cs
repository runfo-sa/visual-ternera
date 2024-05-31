using Comparator.Model;
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
        }

        private void firstTextEditor_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            secondTextEditor.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void secondTextEditor_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            firstTextEditor.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void firstTextEditor_Loaded(Object sender, RoutedEventArgs e)
        {
            Color leftColor = Color.FromRgb(189, 51, 95);
            Color rightColor = Color.FromRgb(51, 189, 104);

            var firstLines = firstTextEditor.Document!.Text.Split(Environment.NewLine);
            var secondLines = secondTextEditor.Document!.Text.Split(Environment.NewLine);

            List<int> lines = [];

            int i = 0, j = 0;
            for (; i < firstLines.Length && j < secondLines.Length; i++, j++)
            {
                if (firstLines[i] != secondLines[j])
                {
                    lines.Add(i + 1);
                }
            }

            if (i >= firstLines.Length && j < secondLines.Length)
            {
                firstTextEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(lines, leftColor));
                lines.AddRange(Enumerable.Range(j, secondLines.Length));
                secondTextEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(lines, rightColor));
            }
            else if (i < firstLines.Length && j >= secondLines.Length)
            {
                secondTextEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(lines, rightColor));
                lines.AddRange(Enumerable.Range(i, firstLines.Length));
                firstTextEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(lines, leftColor));
            }
            else
            {
                firstTextEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(lines, leftColor));
                secondTextEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(lines, rightColor));
            }
        }
    }
}
