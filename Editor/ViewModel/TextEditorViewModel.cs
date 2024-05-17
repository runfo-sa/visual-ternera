using ICSharpCode.AvalonEdit;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Editor.ViewModel
{
    public class TextEditorViewModel : TextEditor, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty CarretOffsetProperty =
            DependencyProperty.Register("CarretOffset", typeof(int), typeof(TextEditorViewModel));
    }
}
