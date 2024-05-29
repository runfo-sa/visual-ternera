using Core;
using Editor.Model;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Editor.ViewModel
{
    public class EditorViewModel : ViewModelBase
    {
        private int _newTabNumber = 0;

        public PreviewViewModel PreviewViewModel { get; }

        public ObservableCollection<TabItem> Tabs { get; set; } = [];
        public ObservableCollection<object> RootDir { get; set; } = [];

        private TabItem _unpinnedTab = new("", new TextDocument(""))
        {
            Visibility = Visibility.Hidden
        };

        public TabItem UnpinnedTab
        {
            get => _unpinnedTab;
            set
            {
                _unpinnedTab = value;
                _unpinnedTab.IsModified += PinUnpinned;
                OnPropertyChanged(nameof(UnpinnedTab));
            }
        }

        private int _selectedTab = 0;

        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        private readonly CollectionView _printers = new(PrinterSettings.InstalledPrinters);

        public CollectionView Printers => _printers;

        public RelayCommand OpenFile => new(Open);
        public RelayCommand<TabItem> CloseTab => new(Close);

        public RelayCommand PreviewLabel => new(
            _ => PreviewViewModel.GeneratePreview(SelectedTab == 0 ? UnpinnedTab : Tabs[SelectedTab - 1]),
            _ => (SelectedTab == 0) ? UnpinnedTab.Visibility == Visibility.Visible : (Tabs.Count > 0 && Tabs[SelectedTab - 1] != null)
        );

        public RelayCommand NewFile => new(_ => AddTab($"new {++_newTabNumber}", "^XA\r\n\r\n^XZ"));

        public RelayCommand SaveFile => new(
            _ => Tabs[SelectedTab - 1].SaveItem(),
            _ => SelectedTab > 0 && Tabs[SelectedTab - 1] is not null && Tabs[SelectedTab - 1].HasUnsavedChanges
        );

        public RelayCommand OpenSelected => new(label =>
        {
            if (label is not null && label is LabelFile)
            {
                var file = label as LabelFile;
                var select = Tabs.FirstOrDefault(t => t.Header == file!.Name);
                if (select is not null)
                {
                    SelectedTab = Tabs.IndexOf(select) + 1;
                }
                else
                {
                    var content = File.ReadAllText(file!.Path);
                    AddUnpinnedTab(file.Name, content, file.Path);
                }
            }
        });

        public RelayCommand SaveAllFiles => new(_ =>
        {
            foreach (var item in Tabs)
            {
                if (item.HasUnsavedChanges)
                {
                    item.SaveItem();
                }
            }
        }, p => Tabs.Count > 0);

        public RelayCommand PinTab => new(_ =>
        {
            AddTab(UnpinnedTab.Header, UnpinnedTab.EditorBody.Text, UnpinnedTab.Path);
            Close(UnpinnedTab);
        });

        public RelayCommand Print => new(_ =>
        {
            var tab = SelectedTab == 0 ? UnpinnedTab : Tabs[SelectedTab - 1];
            var labelary = new Labelary(tab.EditorBody.Text).FillVariables(Settings);

            PrinterHelper.SendStringToPrinter(Printers.CurrentItem.ToString()!, labelary.Content, tab.Header);
        }, _ => (SelectedTab == 0) ? UnpinnedTab.Visibility == Visibility.Visible : (Tabs.Count > 0 && Tabs[SelectedTab - 1] != null));

        public EditorViewModel(Settings settings) : base(settings)
        {
            PreviewViewModel = new(settings);
            InitTree();
        }

        private void InitTree()
        {
            var caja = new LabelDir("Caja");
            var otro = new LabelDir("Otros");
            var prim = new LabelDir("Primaria");

            var files = Directory.GetFiles(Settings.EtiquetasDir, "*.e01");
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                if (filename.StartsWith("CAJA", StringComparison.CurrentCultureIgnoreCase))
                {
                    caja.Labels.Add(new LabelFile(file));
                }
                else if (filename.StartsWith("PRIMARIA", StringComparison.CurrentCultureIgnoreCase))
                {
                    prim.Labels.Add(new LabelFile(file));
                }
                else
                {
                    otro.Labels.Add(new LabelFile(file));
                }
            }

            RootDir.Add(caja);
            RootDir.Add(prim);
            RootDir.Add(otro);
        }

        private void Open(object? obj)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "ZPL File (*.e01)|*.e01|Todos los archivos (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var content = File.ReadAllText(dialog.FileName);
                AddTab(dialog.SafeFileName, content, dialog.FileName);
            }
        }

        private void Close(TabItem? tab)
        {
            if (tab is not null)
            {
                if (tab.HasUnsavedChanges)
                {
                    switch (MessageBox.Show($"Guardar archivo '{tab.Header}'?", "Guardar", MessageBoxButton.YesNoCancel))
                    {
                        case MessageBoxResult.Yes:
                            tab.SaveItem();
                            Tabs.Remove(tab);
                            break;

                        case MessageBoxResult.No:
                            Tabs.Remove(tab);
                            break;

                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
                else if (tab == UnpinnedTab)
                {
                    UnpinnedTab = new TabItem("", new TextDocument(""))
                    {
                        Visibility = Visibility.Hidden
                    };
                }
                else
                {
                    Tabs.Remove(tab);
                }
            }
        }

        private void AddUnpinnedTab(string header, string content, string? path = null)
        {
            AddTab(header, content, path, false);
        }

        private void AddTab(string header, string content, string? path = null, bool pinned = true)
        {
            if (pinned)
            {
                Tabs.Add(new TabItem(header, new TextDocument(content), path, pinned));
                SelectedTab = Tabs.Count;
            }
            else
            {
                UnpinnedTab = new TabItem(header, new TextDocument(content), path, pinned)
                {
                    Visibility = Visibility.Visible
                };
                SelectedTab = 0;
            }
        }

        private void PinUnpinned(object? sender, EventArgs e)
        {
            var tab = new TabItem(UnpinnedTab.Header + '*', new TextDocument(UnpinnedTab.EditorBody.Text), UnpinnedTab.Path)
            {
                HasUnsavedChanges = true
            };
            Close(UnpinnedTab);

            Tabs.Add(tab);
            SelectedTab = Tabs.Count;
        }
    }
}
