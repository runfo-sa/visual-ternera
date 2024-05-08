using Core;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System.IO;

namespace Editor.Model
{
    public class TabItem : ObservableObject
    {
        public string? Path { get; set; }
        public bool HasUnsavedChanes { get; set; } = false;

        private string _header;

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public TextDocument EditorBody { get; set; }

        public TabItem(string header, TextDocument editorBody, string? path = null)
        {
            Path = path;
            _header = header;
            EditorBody = editorBody;
            EditorBody.TextChanged += UnsavedChanges;
        }

        private void UnsavedChanges(object? sender, EventArgs e)
        {
            Header += '*';
            HasUnsavedChanes = true;
            EditorBody.TextChanged -= UnsavedChanges;
        }

        public static void SaveItem(TabItem? item)
        {
            if (item is not null && item.HasUnsavedChanes)
            {
                if (item.Path is not null)
                {
                    File.WriteAllText(item.Path, item.EditorBody.Text);
                    item.Header = item.Header.Remove(item.Header.LastIndexOf('*'));
                }
                else
                {
                    SaveFileDialog dialog = new()
                    {
                        Filter = "ZPL File (*.e01)|*.e01|Todos los archivos (*.*)|*.*"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        File.WriteAllText(dialog.FileName, item.EditorBody.Text);
                        item.Header = dialog.SafeFileName;
                        item.Path = dialog.FileName;
                    }
                }

                item.EditorBody.TextChanged += item.UnsavedChanges;
                item.HasUnsavedChanes = false;
            }
        }
    }
}
