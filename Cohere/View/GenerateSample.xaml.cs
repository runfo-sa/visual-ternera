using AdonisUI.Controls;
using Core;
using Core.Database;
using Core.Database.Model;
using Core.Logic;
using Core.MVVM;
using Core.ViewLogic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Cohere.View
{
    public partial class GenerateSample : AdonisWindow, INotifyPropertyChanged
    {
        public class ProductoMuestra(string codigo, string nombre, string senasa, bool muestra) : ObservableObject
        {
            public string Codigo { get; set; } = codigo;
            public string Nombre { get; set; } = nombre;
            public string Senasa { get; set; } = senasa;

            private bool _muestra = muestra;

            public bool Muestra
            {
                get => _muestra;
                set
                {
                    _muestra = value;
                    OnPropertyChanged(nameof(Muestra));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ProductoMuestra> ProductsList { get; set; }

        private readonly CollectionView _printers = new(PrinterSettings.InstalledPrinters);
        public CollectionView Printers => _printers;

        private bool _selectAll;

        public bool SelectAll
        {
            get => _selectAll;
            set
            {
                _selectAll = value;
                SelectedAll();
                OnPropertyChanged(nameof(SelectAll));
            }
        }

        public RelayCommand AceptDialog => new(_ =>
        {
            if (generarRecall.IsChecked == true)
            {
                GenerateRecall(ProductsList);
            }
            else
            {
                PrintLabels(ProductsList);
            }

            DialogResult = true;
        }, _ => ProductsList.Any(p => p.Muestra));

        private readonly Settings _settings;
        private readonly LabelFile _labelFile;

        public GenerateSample(Settings settings, string etiqueta, LabelFile labelFile)
        {
            InitializeComponent();
            DataContext = this;
            _settings = settings;
            _labelFile = labelFile;

            using (var context = new IdeDbContext(settings.SqlConnection))
            {
                var parameter = new SqlParameter("@Etiqueta", etiqueta);
                ProductsList = [.. context.Database
                    .SqlQueryRaw<ListarProductos>("ide.ListarProductos @Etiqueta", parameter)
                    .AsEnumerable()
                    .Select(p => new ProductoMuestra(p.Codigo, p.Nombre, p.Senasa, false))];
            }
        }

        private void SelectedAll()
        {
            foreach (var item in ProductsList)
            {
                item.Muestra = SelectAll;
            }
        }

        private void PrintLabels(IEnumerable<ProductoMuestra> products)
        {
            foreach (var prod in products)
            {
                if (prod.Muestra)
                {
                    var label = new Labelary(File.ReadAllText(_labelFile.Path))
                        .FillProduct(_settings, prod.Codigo)
                        .FillVariables(_settings);
                    PrinterHelper.SendStringToPrinter(Printers.CurrentItem.ToString()!, label.Content, $"{_labelFile.Name} - {prod.Nombre}");
                }
            }
        }

        private void GenerateRecall(IEnumerable<ProductoMuestra> products)
        {
            GenerateRecallDialog dialog = new(_settings, products);
            if (dialog.ShowDialog() == true)
            {
                PrintLabels(products);
            }
        }
    }
}
