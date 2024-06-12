using Cohere.View;
using Core;
using Core.Database;
using Core.Database.Model;
using Core.Logic;
using Core.MVVM;
using Core.ViewLogic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.Serialization;

namespace Cohere.ViewModel
{
    public class CohereViewModel : ViewModelBase
    {
        public ObservableCollection<object> RootDir { get; set; } = [];
        public ObservableCollection<ListarProductos> ProductsList { get; set; } = [];
        public ObservableCollection<Valor> SelectedValues { get; set; } = [];

        public int FoundErrors
        {
            get => ProductsList.Where(p => p.Error != ProductoError.Ninguno).Count();
        }

        private string _reglaNombre = string.Empty;

        public string ReglaNombre
        {
            get => _reglaNombre;
            set
            {
                _reglaNombre = value;
                OnPropertyChanged(nameof(ReglaNombre));
            }
        }

        private LabelFile _currentLabel = null!;

        public LabelFile CurrentLabel
        {
            get => _currentLabel;
            set
            {
                _currentLabel = value;
                OnPropertyChanged(nameof(CurrentLabel));
            }
        }

        public RelayCommand OpenSelected => new(label =>
        {
            if (label is not null && label is LabelFile)
            {
                var file = label as LabelFile;
                var etiquetaName = file!.Name.Replace(".e01", "", StringComparison.CurrentCultureIgnoreCase);

                CurrentLabel = file;

                using (var context = new IdeDbContext(Settings.SqlConnection))
                {
                    var parameter = new SqlParameter("@Etiqueta", etiquetaName);
                    var result = context.Database
                        .SqlQueryRaw<ListarProductos>("ide.ListarProductos @Etiqueta", parameter)
                        .ToList();

                    ProductsList.Clear();
                    foreach (var item in result)
                    {
                        ProductsList.Add(item);
                    }

                    var regla = context.Reglas.FirstOrDefault(r => r.Etiqueta == etiquetaName);
                    if (regla is not null)
                    {
                        ReglaNombre = regla.Nombre;
                        IEnumerable<ReglaAtributo> reglasAtributos = [.. context.ReglasAtributos];
                        foreach (var atributo in reglasAtributos)
                        {
                            var etiqueta = new SqlParameter("@Etiqueta", etiquetaName);
                            var atrib = new SqlParameter("@Var", atributo.AtributoNombre);

                            var rc = context.Database
                                .SqlQueryRaw<ReglaKeyValue>("ide.BuscarReglaAtributo @Etiqueta, @Var", etiqueta, atrib)
                                .ToList();

                            foreach (var item in rc)
                            {
                                var product = ProductsList.First(p => p.Codigo == item.Codigo);

                                if (atributo.EsAtributoEstatico && item.Valor is not null && item.Valor != atributo.ValorEstatico)
                                {
                                    product.Error = ProductoError.Incoherente;
                                }
                                else if (item.Valor is null)
                                {
                                    product.Error = ProductoError.Incompleto;
                                }

                                product.Valores.Add(new Valor(atributo.AtributoNombre, item.Valor, product.Error));
                            }
                        }
                    }
                    else
                    {
                        ReglaNombre = "No Existente";
                    }
                }

                OnPropertyChanged(nameof(FoundErrors));
            }
        });

        public RelayCommand LoadSelected => new(product =>
        {
            if (product is not null && product is ListarProductos)
            {
                SelectedValues.Clear();

                var prod = product as ListarProductos;
                foreach (var valor in prod!.Valores)
                {
                    SelectedValues.Add(valor);
                }
            }
        });

        public RelayCommand ChangeRule => new(label =>
        {
            if (label is not null && label is LabelFile)
            {
                var file = label as LabelFile;
                SelectRuleDialog dialog = new(Settings);

                if (dialog.ShowDialog() == true)
                {
                    var etiquetaName = file!.Name.Replace(".e01", "", StringComparison.CurrentCultureIgnoreCase);
                    using (var context = new IdeDbContext(Settings.SqlConnection))
                    {
                        var regla = context.Reglas.FirstOrDefault(r => r.Etiqueta == etiquetaName);
                        if (regla is not null)
                        {
                            regla.Nombre = dialog.SelectedRule.Nombre;
                        }
                        else
                        {
                            context.Reglas.Add(new Regla()
                            {
                                Nombre = dialog.SelectedRule.Nombre,
                                Etiqueta = etiquetaName
                            });
                        }
                        context.SaveChanges();
                        ReglaNombre = dialog.SelectedRule.Nombre;
                    }
                }
            }
        });

        public RelayCommand CreateRule => new(_ =>
        {
            CreateRuleDialog dialog = new(Settings);
            if (dialog.ShowDialog() == true)
            {
                using (var context = new IdeDbContext(Settings.SqlConnection))
                {
                    var regla = context.Reglas.FirstOrDefault(r => r.Etiqueta == dialog.Etiqueta);
                    if (regla is not null)
                    {
                        regla.Nombre = dialog.ReglaNombre;
                    }
                    else
                    {
                        regla = context.Reglas.Add(new Regla()
                        {
                            Nombre = dialog.ReglaNombre,
                            Etiqueta = dialog.Etiqueta
                        }).Entity;
                    }
                    context.SaveChanges();
                    ReglaNombre = dialog.ReglaNombre;

                    foreach (var attr in dialog.Atributos)
                    {
                        attr.Reglas_Id = regla.Id;
                        context.ReglasAtributos.Add(attr);
                    }
                    context.SaveChanges();
                }
            }
        });

        public RelayCommand GenerarMuestra => new(_ =>
        {
            var etiquetaName = CurrentLabel.Name.Replace(".e01", "", StringComparison.CurrentCultureIgnoreCase);
            GenerateSample dialog = new(Settings, etiquetaName, CurrentLabel);
            if (dialog.ShowDialog() == true)
            {
            }
        }, _ => ProductsList.Count > 0 && FoundErrors == 0);

        public CohereViewModel(Core.Logic.Settings settings) : base(settings)
        {
            InitTree();
        }

        private void InitTree()
        {
            var caja = new VirtualDirectory("Caja");
            var otro = new VirtualDirectory("Otros");
            var prim = new VirtualDirectory("Primaria");

            var files = Directory.GetFiles(Settings.EtiquetasDir, "*.e01");
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                if (filename.StartsWith("CAJA", StringComparison.CurrentCultureIgnoreCase))
                {
                    caja.Files.Add(new LabelFile(file));
                }
                else if (filename.StartsWith("PRIMARIA", StringComparison.CurrentCultureIgnoreCase))
                {
                    prim.Files.Add(new LabelFile(file));
                }
                else
                {
                    otro.Files.Add(new LabelFile(file));
                }
            }

            RootDir.Add(caja);
            RootDir.Add(prim);
            RootDir.Add(otro);
        }
    }
}
