using AdonisUI.Controls;
using Cohere.ViewModel;
using Comparator.ViewModel;
using Core;
using Editor.ViewModel;
using Main.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string EditorTitle { get; } = Editor.Editor.Title;
        public RelayCommand OpenEditor { get; set; }

        public string CohereTitle { get; } = Cohere.Cohere.Title;
        public RelayCommand OpenCohere { get; set; }

        public string ComparatorTitle { get; } = Comparator.Comparator.Title;
        public RelayCommand OpenComparator { get; set; }

        public ObservableCollection<Client> ClientsList { get; set; }

        private int _lastRefreshed = 0;

        public int LastRefreshed
        {
            get => _lastRefreshed;
            set
            {
                _lastRefreshed = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand RefreshClients { get; set; }
        public GitTag Tag { get; set; }

        private bool _darkTheme = true;

        public bool DarkTheme
        {
            get => _darkTheme;
            set
            {
                _darkTheme = value;
                OnPropertyChanged(nameof(DarkTheme));
            }
        }

        private BitmapImage _themeIcon = new(new Uri("/Main;component/Resources/dark-theme.png", UriKind.Relative));

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
            DarkTheme = !DarkTheme;

            var uri = (DarkTheme) ? "/Main;component/Resources/dark-theme.png" : "/Main;component/Resources/light-theme.png";
            ThemeIcon = new BitmapImage(new Uri(uri, UriKind.Relative));

            ((App)Application.Current).ChangeTheme(DarkTheme);
        });

        public MainViewModel(Settings settings) : base(settings)
        {
            var dbContext = new ClientDbContext(Settings.SqlConnection);
            ClientsList = [.. dbContext.EstadoCliente];

            OpenEditor = OpenWindow<Editor.Editor, EditorViewModel>(Settings);
            OpenCohere = OpenWindow<Cohere.Cohere, CohereViewModel>(Settings);
            OpenComparator = OpenWindow<Comparator.Comparator, ComparatorViewModel>(Settings);
            RefreshClients = new RelayCommand(a => UpdateClients(), p => true);

            DispatcherTimer refreshTimer = new()
            {
                Interval = TimeSpan.FromMinutes(60)
            };
            refreshTimer.Tick += RefreshTimer;
            refreshTimer.Start();

            DispatcherTimer updateTime = new()
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            updateTime.Tick += UpdateTime;
            updateTime.Start();

            string rc = GitTag.RunGitCommand(
                "for-each-ref",
                "--format=\"%(refname:short)|%(creatordate:format:%Y/%m/%d %I:%M)|%(subject)\" \"refs/tags/*\"",
                "C:\\Users\\Agustin.Marco\\Projects\\Apps\\C#\\visual_ternera\\IDE\\TestRepo"
            );
            Tag = GitTag.Parse(rc);
        }

        private static RelayCommand OpenWindow<T, K>(Settings settings)
            where T : IContent, new()
            where K : ViewModelBase
        {
            return new RelayCommand(a =>
            {
                new AdonisWindow
                {
                    Content = new T() { },
                    Title = "Visual Ternera - " + T.Title,
                    DataContext = Activator.CreateInstance(typeof(K), settings)
                }.Show();
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
            var dbContext = new ClientDbContext(Settings.SqlConnection);
            foreach (var client in dbContext.EstadoCliente)
            {
                ClientsList.Add(client);
            }
        }
    }
}
