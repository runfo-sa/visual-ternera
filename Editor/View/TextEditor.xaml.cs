using System.Windows;
using System.Windows.Controls;

namespace Editor.View
{
    public partial class TextEditor : UserControl
    {
        public static readonly DependencyProperty EditorBodyProperty = DependencyProperty.Register("EditorBody", typeof(string), typeof(TextEditor));

        public string EditorBody
        {
            get { return (String)GetValue(EditorBodyProperty); }
            set { SetValue(EditorBodyProperty, value); }
        }

        private int LineCounter = 1;

        public TextEditor()
        {
            InitializeComponent();
            lineCount.Text = $"{LineCounter}{Environment.NewLine}";
        }

        private void TextBox_TextChanged(Object sender, TextChangedEventArgs e)
        {
            var count = textBox.Text.Count(n => n == '\n') + 1;

            if (count == LineCounter)
            {
                return;
            }

            if (count < LineCounter)
            {
                LineCounter = count;
                lineCount.Text = lineCount.Text.Remove(lineCount.Text.IndexOf($"{LineCounter + 1}{Environment.NewLine}"));
            }
            else if (count > LineCounter)
            {
                LineCounter = count;
                lineCount.Text += $"{LineCounter}{Environment.NewLine}";
            }
        }

        private void TextBox_SelectionChanged(Object sender, System.Windows.RoutedEventArgs e)
        {
            int row = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
            int col = textBox.CaretIndex - textBox.GetCharacterIndexFromLineIndex(row);
            lblCursorPosition.Text = $"Line: {row + 1}    Char: {col + 1}";
        }
    }
}
