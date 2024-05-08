using Core;
using Editor.Model;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Editor.ViewModel
{
    public class EditorViewModel : ViewModelBase
    {
        public ObservableCollection<TabItem> Tabs { get; set; } = [];
        public ObservableCollection<object> RootDir { get; set; } = [];

        public CollectionView DpiList { get; }
        public CollectionView SizeList { get; }

        private int _newTabNumber = 0;
        private int _selectedTab = 0;

        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        private BitmapSource _previewImage = null!;

        public BitmapSource PreviewImage
        {
            get => _previewImage;
            set
            {
                _previewImage = value;
                OnPropertyChanged();
            }
        }

        private double _previewAngle = 0.0;

        public double PreviewAngle
        {
            get => _previewAngle;
            set
            {
                _previewAngle = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OpenFile => new(Open);
        public RelayCommand<TabItem> CloseTab => new(Close);
        public RelayCommand<TabItem> PreviewLabel => new(GeneratePreview, p => p is not null);
        public RelayCommand NewFile => new(_ => AddTab($"new {++_newTabNumber}", "^XA\r\n\r\n^XZ"));
        public static RelayCommand<TabItem> SaveFile => new(TabItem.SaveItem, p => p is not null && p.HasUnsavedChanes);

        public RelayCommand<LabelFile> OpenSelected => new(label =>
        {
            if (label is not null)
            {
                var select = Tabs.FirstOrDefault(t => t.Header == label.Name);
                if (select is not null)
                {
                    SelectedTab = Tabs.IndexOf(select);
                }
                else
                {
                    var content = File.ReadAllText(label.Path);
                    AddTab(label.Name, content, label.Path);
                }
            }
        });

        public RelayCommand SaveAllFiles => new(_ =>
        {
            foreach (var item in Tabs)
            {
                if (item.HasUnsavedChanes)
                {
                    TabItem.SaveItem(item);
                }
            }
        }, p => Tabs.Count > 0);

        public RelayCommand RotateRight => new(_ =>
        {
            PreviewImage = new TransformedBitmap(PreviewImage, new RotateTransform(90.0));
            var angle = PreviewAngle + 90.0;
            PreviewAngle = angle >= 360.0 ? 0.0 : angle;
        }, p => PreviewImage is not null);

        public RelayCommand RotateLeft => new(_ =>
        {
            PreviewImage = new TransformedBitmap(PreviewImage, new RotateTransform(-90.0));
            var angle = PreviewAngle - 90.0;
            PreviewAngle = angle < 0.0 ? 270.0 : angle;
        }, p => PreviewImage is not null);

        public RelayCommand ZoomIn => new(_ =>
        {
            PreviewImage = new TransformedBitmap(PreviewImage, new ScaleTransform(0.75, 0.75));
        });

        public RelayCommand ZoomOut => new(_ =>
        {
            PreviewImage = new TransformedBitmap(PreviewImage, new ScaleTransform(1.25, 1.25));
        });

        public EditorViewModel(Settings settings) : base(settings)
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

            SizeList = new(SizeConstants.All);
            DpiList = new(DpiConstants.All);
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
                if (tab.HasUnsavedChanes)
                {
                    switch (MessageBox.Show($"Guardar archivo '{tab.Header}'?", "Guardar", MessageBoxButton.YesNoCancel))
                    {
                        case MessageBoxResult.Yes:
                            TabItem.SaveItem(tab);
                            Tabs.Remove(tab);
                            break;

                        case MessageBoxResult.No:
                            Tabs.Remove(tab);
                            break;

                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
                else
                {
                    Tabs.Remove(tab);
                }
            }
        }

        private void AddTab(string header, string content, string? path = null)
        {
            Tabs.Add(new TabItem(header, new TextDocument(content), path));
            SelectedTab = Tabs.Count - 1;
        }

        private async void GeneratePreview(TabItem? item)
        {
            var bytes = await new Labelary().Post(item!.EditorBody.Text, ((Dpi)DpiList.CurrentItem).Value, ((Model.Size)SizeList.CurrentItem).Value);
            if (bytes is not null)
            {
                using MemoryStream stream = new(bytes);
                PreviewImage = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
    }
}
