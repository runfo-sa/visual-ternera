using Core.MVVM;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Editor.Model
{
    public class TabItem : ObservableObject
    {
        public event EventHandler? IsModified;

        public TextDocument EditorBody { get; set; }
        public string? Path { get; set; }
        public bool Pinned { get; set; } = false;

        private bool _hasUnsavedChanges = false;

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set
            {
                _hasUnsavedChanges = value;
                OnPropertyChanged(nameof(HasUnsavedChanges));
            }
        }

        private string _header = null!;

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        private Visibility _visibility;

        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        private List<LintingInfo> _lintingData = [];

        public List<LintingInfo> LintingData
        {
            get => _lintingData;
            set
            {
                _lintingData = value;
                OnPropertyChanged(nameof(LintingData));
            }
        }

        public TabItem(string header, TextDocument editorBody, string? path = null, bool pinned = true)
        {
            Path = path;
            Pinned = pinned;
            Header = header;
            EditorBody = editorBody;
            EditorBody.TextChanged += UnsavedChanges;
            EditorBody.UndoStack.PropertyChanged += ResetChanges;
        }

        private void UnsavedChanges(object? sender, EventArgs e)
        {
            if (!Pinned)
            {
                IsModified?.Invoke(this, EventArgs.Empty);
            }
            else if (!HasUnsavedChanges)
            {
                Header += '*';
                HasUnsavedChanges = true;
                EditorBody.TextChanged -= UnsavedChanges;
            }
        }

        private void ResetChanges(object? sender, PropertyChangedEventArgs e)
        {
            if (EditorBody.UndoStack.CanUndo == false && HasUnsavedChanges)
            {
                Header = Header[..(Header.Length - 1)];
                EditorBody.TextChanged += UnsavedChanges;
                HasUnsavedChanges = false;
            }
        }

        public void SaveItem(string extension)
        {
            if (HasUnsavedChanges)
            {
                if (Path is not null)
                {
                    File.WriteAllText(Path, EditorBody.Text);
                    Header = Header[..(Header.Length - 1)];
                    EditorBody.TextChanged += UnsavedChanges;
                    HasUnsavedChanges = false;
                }
                else
                {
                    SaveFileDialog dialog = new()
                    {
                        Filter = $"ZPL File (*.{extension})|*.{extension}|Todos los archivos (*.*)|*.*"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        File.WriteAllText(dialog.FileName, EditorBody.Text);
                        Header = dialog.SafeFileName;
                        Path = dialog.FileName;
                        EditorBody.TextChanged += UnsavedChanges;
                        HasUnsavedChanges = false;
                    }
                }
            }
        }

        public void ClearLinting()
        {
            LintingData = [];
        }
    }
}
