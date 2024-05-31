﻿using Core;
using Core.Logic;
using Core.MVVM;
using Core.View;
using Core.ViewLogic;
using Editor.Model;
using Editor.View;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace Editor.ViewModel
{
    public class EditorViewModel(Settings settings) : ViewModelBase(settings)
    {
        private bool _doubleClick = false;
        private int _newTabNumber = 0;

        public static CollectionView Printers => new(PrinterSettings.InstalledPrinters);
        public PreviewViewModel PreviewViewModel { get; } = new(settings);
        public ObservableCollection<TabItem> Tabs { get; set; } = [];
        public TreeGenerator Tree { get; } = new(settings);

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

        private bool _previewOnSave = false;

        public bool PreviewOnSave
        {
            get => _previewOnSave;
            set
            {
                _previewOnSave = value;
                OnPropertyChanged(nameof(PreviewOnSave));
            }
        }

        private string? _previewErrors = null;

        public string? PreviewErrors
        {
            get => _previewErrors;
            set
            {
                _previewErrors = value;
                OnPropertyChanged(nameof(PreviewErrors));
            }
        }

        private Visibility _errorsWindow = Visibility.Hidden;

        public Visibility ErrorsWindow
        {
            get => _errorsWindow;
            set
            {
                _errorsWindow = value;
                OnPropertyChanged(nameof(ErrorsWindow));
            }
        }

        public BitmapImage NewIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-new.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public BitmapImage OpenIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-open.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public BitmapImage SaveIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-save.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public BitmapImage SaveAllIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-save-all.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public BitmapImage PreviewIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-preview.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public BitmapImage PrintIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-print.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public RelayCommand OpenFile => new(Open);
        public RelayCommand<TabItem> CloseTab => new(Close);

        public RelayCommand PreviewLabel => new(
            async _ =>
            {
                PreviewErrors = await PreviewViewModel.GeneratePreview(SelectedTab == 0 ? UnpinnedTab : Tabs[SelectedTab - 1]);
                if (!PreviewErrors.IsNullOrEmpty())
                {
                    ErrorsWindow = Visibility.Visible;
                }
            },
            _ => (SelectedTab == 0) ? UnpinnedTab.Visibility == Visibility.Visible : (Tabs.Count > 0 && Tabs[SelectedTab - 1] != null)
        );

        public RelayCommand NewFile => new(_ => AddTab($"new {++_newTabNumber}", "^XA\r\n\r\n^XZ"));

        public RelayCommand SaveFile => new(
            _ =>
            {
                Tabs[SelectedTab - 1].SaveItem(Settings.EtiquetasExtension);
                if (PreviewOnSave)
                {
                    PreviewLabel.Execute(this);
                }
            },
            _ => SelectedTab > 0 && Tabs[SelectedTab - 1] is not null && Tabs[SelectedTab - 1].HasUnsavedChanges
        );

        public RelayCommand SaveAsFile => new(
            _ =>
            {
                var tab = Tabs[SelectedTab - 1];
                tab.Path = null;
                tab.SaveItem(Settings.EtiquetasExtension);
                if (PreviewOnSave)
                {
                    PreviewLabel.Execute(this);
                }
            },
            _ => SelectedTab > 0 && Tabs[SelectedTab - 1] is not null && Tabs[SelectedTab - 1].HasUnsavedChanges
        );

        public RelayCommand OpenSelected => new(item =>
        {
            if (item is not null && item is LabelFile label)
            {
                DispatcherTimer doubleClickTimer = new()
                {
                    Interval = TimeSpan.FromMilliseconds(200)
                };
                doubleClickTimer.Tick += (sender, e) =>
                {
                    OpenTab(label);
                    doubleClickTimer.Stop();
                    _doubleClick = false;
                };
                doubleClickTimer.Start();
            }
        });

        public RelayCommand OpenSelectedPinned => new(item =>
        {
            if (item is not null && item is LabelFile)
            {
                _doubleClick = true;
            }
        });

        public RelayCommand SaveAllFiles => new(_ =>
        {
            foreach (var item in Tabs)
            {
                if (item.HasUnsavedChanges)
                {
                    item.SaveItem(Settings.EtiquetasExtension);
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

        public static RelayCommand AboutPopUp => new(_ =>
        {
            AboutPopUp popUp = new();
            popUp.ShowDialog();
        });

        public static RelayCommand HelpPopUp => new(_ =>
        {
            Help popUp = new();
            popUp.ShowDialog();
        });

        public static RelayCommand OpenSettings => new(_ =>
        {
            SettingsWindow window = new();
            window.ShowDialog();
        });

        public static RelayCommand DownSize => new(_ =>
        {
            PreviewViewModel.DownSize();
        });

        public static RelayCommand UpSize => new(_ =>
        {
            PreviewViewModel.UpSize();
        });

        public RelayCommand CloseErrorWindow => new(_ =>
        {
            ErrorsWindow = Visibility.Hidden;
            PreviewErrors = null;
        });

        private void Open(object? obj)
        {
            OpenFileDialog dialog = new()
            {
                Filter = $"ZPL File (*.{Settings.EtiquetasExtension})|*.{Settings.EtiquetasExtension}|Todos los archivos (*.*)|*.*"
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
                            tab.SaveItem(Settings.EtiquetasExtension);
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
            UnpinnedTab.IsModified -= PinUnpinned;

            var tab = new TabItem(UnpinnedTab.Header + '*', UnpinnedTab.EditorBody, UnpinnedTab.Path)
            {
                HasUnsavedChanges = true
            };

            Close(UnpinnedTab);
            Tabs.Add(tab);
            SelectedTab = Tabs.Count;
        }

        private void OpenTab(LabelFile label)
        {
            var selected = Tabs.FirstOrDefault(t => t.Header == label!.Name);
            if (selected is not null)
            {
                SelectedTab = Tabs.IndexOf(selected) + 1;
            }
            else
            {
                var content = File.ReadAllText(label!.Path);
                if (_doubleClick)
                {
                    AddTab(label.Name, content, label.Path);
                }
                else
                {
                    AddUnpinnedTab(label.Name, content, label.Path);
                }
            }
        }
    }
}
