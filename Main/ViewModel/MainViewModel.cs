using AdonisUI.Controls;
using Cohere.ViewModel;
using Comparator.ViewModel;
using Core;
using Core.Git;
using Core.Interfaces;
using Core.Logic;
using Core.Logic.SettingsModel;
using Core.MVVM;
using Editor.ViewModel;
using Main.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using YamlDotNet.Serialization;

namespace Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public static string EditorTitle => Editor.Editor.Title;
        public static string CohereTitle => Cohere.Cohere.Title;
        public static string ComparatorTitle => Comparator.Comparator.Title;
        public ObservableCollection<Client> ClientsList { get; set; }
        public GitTag GitTag { get; set; }
        public string GitTagUri { get; set; }

        private int _lastRefreshed = 0;

        public int LastRefreshed
        {
            get => _lastRefreshed;
            set
            {
                _lastRefreshed = value;
                OnPropertyChanged(nameof(LastRefreshed));
            }
        }

        private BitmapImage _themeIcon = null!;

        public BitmapImage ThemeIcon
        {
            get => _themeIcon;
            set
            {
                _themeIcon = value;
                OnPropertyChanged(nameof(ThemeIcon));
            }
        }

        public RelayCommand ChangeTheme => new(_ =>
        {
            Settings.Theme = Settings.Theme switch
            {
                Theme.Dark => Theme.Light,
                Theme.Light => Theme.Dark,
                _ => throw new NotImplementedException()
            };

            var uri = Settings.Theme switch
            {
                Theme.Dark => "/Main;component/Resources/dark-theme.png",
                Theme.Light => "/Main;component/Resources/light-theme.png",
                _ => throw new NotImplementedException(),
            };

            ThemeIcon = new BitmapImage(new Uri(uri, UriKind.Relative));
            ((App)Application.Current).ChangeTheme(Settings.Theme);

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(Settings);
            File.WriteAllText("Settings.yaml", yaml);
        });

        public RelayCommand OpenEditor => OpenWindow<Editor.Editor, EditorViewModel>(Settings);
        public RelayCommand OpenCohere => OpenWindow<Cohere.Cohere, CohereViewModel>(Settings);
        public RelayCommand OpenComparator => OpenWindow<Comparator.Comparator, ComparatorViewModel>(Settings);
        public RelayCommand RefreshClients => new(_ => UpdateClients());

        public MainViewModel(Core.Logic.Settings settings) : base(settings)
        {
            ThemeIcon = new(new Uri($"/Main;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-theme.png", UriKind.Relative));

            var dbContext = new ServiceDbContext(Settings.SqlConnection);
            ClientsList = [.. dbContext.EstadoCliente];

            // Reloj que refresca la lista de clientes cada 30 minutos
            DispatcherTimer refreshTimer = new()
            {
                Interval = TimeSpan.FromMinutes(30)
            };
            refreshTimer.Tick += RefreshTimer;
            refreshTimer.Start();

            // Reloj que actualiza el tiempo pasado desde la ultima actualizacion
            DispatcherTimer updateTime = new()
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            updateTime.Tick += UpdateTime;
            updateTime.Start();

            var tags = Git.RunGitCommand(
                "for-each-ref",
                "--format=\"%(refname:short)|%(creatordate:format:%Y/%m/%d %I:%M)|%(subject)\\n\" \"refs/tags/*\"",
                Settings.EtiquetasDir
            ).Split("\\n", StringSplitOptions.RemoveEmptyEntries).Select(GitTag.Parse);

            if (!Settings.GitRepo.EndsWith('/'))
            {
                Settings.GitRepo += '/';
            }

            GitTag = tags.Last();
            GitTagUri = $"{Settings.GitRepo}releases/tag/{GitTag.Tag}";
        }

        private static RelayCommand OpenWindow<T, K>(Core.Logic.Settings settings)
            where T : IContent, new()
            where K : ViewModelBase
        {
            return new RelayCommand(a =>
            {
                var vm = Activator.CreateInstance(typeof(K), settings) as ViewModelBase;
                var win = new AdonisWindow
                {
                    Content = new T() { },
                    Title = "Visual Ternera - " + T.Title,
                    DataContext = vm
                };

                if (!vm!.Closed)
                {
                    win.Show();
                }
            });
        }

        private void UpdateTime(object? sender, EventArgs args)
        {
            LastRefreshed++;
        }

        private void RefreshTimer(object? sender, EventArgs args)
        {
            UpdateClients();
        }

        private void UpdateClients()
        {
            LastRefreshed = 0;
            ClientsList.Clear();
            var dbContext = new ServiceDbContext(Settings.SqlConnection);
            foreach (var client in dbContext.EstadoCliente)
            {
                ClientsList.Add(client);
            }
        }
    }
}
