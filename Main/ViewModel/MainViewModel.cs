using Core;
using Main.Model;
using System.Collections.ObjectModel;
using System.Windows;
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

        public MainViewModel()
        {
            var dbContext = new ClientDbContext();
            ClientsList = [.. dbContext.EstadoCliente];

            OpenEditor = OpenWindow<Editor.Editor>();
            OpenCohere = OpenWindow<Cohere.Cohere>();
            OpenComparator = OpenWindow<Comparator.Comparator>();
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
        }

        private static RelayCommand OpenWindow<T>() where T : IContent, new()
        {
            return new RelayCommand(a =>
            {
                new Window
                {
                    Content = new T(),
                    Title = "Visual Ternera - " + T.Title,
                }.Show();
            }, p => true);
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
            var dbContext = new ClientDbContext();
            foreach (var client in dbContext.EstadoCliente)
            {
                ClientsList.Add(client);
            }
        }
    }
}