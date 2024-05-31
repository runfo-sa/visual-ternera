using AdonisUI.Controls;
using Core.Database.Model;
using Core.Logic;
using Core.MVVM;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.IO;

namespace Cohere.View
{
    public partial class CreateRuleDialog : AdonisWindow
    {
        public ObservableCollection<ReglaAtributo> Atributos { get; set; } = [];
        public List<string> Etiquetas { get; set; }
        public List<string> AtributosOpciones { get; set; }

        private string _reglaNombre = string.Empty;

        public string ReglaNombre
        {
            get => _reglaNombre;
            set => _reglaNombre = value;
        }

        public string Etiqueta { get; set; } = null!;

        public RelayCommand AceptDialog => new(_ =>
        {
            DialogResult = true;
        }, _ => !reglaNombre.Text.IsNullOrEmpty());

        public CreateRuleDialog(Settings settings)
        {
            InitializeComponent();
            DataContext = this;

            Etiquetas = [.. Directory.GetFiles(settings.EtiquetasDir, "*.e01").Select(p => Path.GetFileNameWithoutExtension(p).ToLower())];

            AtributosOpciones = [
                "Codigo Senasa",
                "Temperatura",
                "Traducciones - Aleman",
                "Traducciones - Ingles",
                "Traducciones - Italiano",
                "Traducciones - Frances",
                "Traducciones - Español",
                "Traducciones - Portugues",
                "Traducciones - Ruso Metro",
                "Traducciones - Ruso",
                "Traducciones - Libre",
                "Traducciones - Chino Hex",
                "Traducciones - Chino",
                "EAN",
                "Definiciones Cuartos - Aleman",
                "Definiciones Cuartos - Ingles",
                "Definiciones Cuartos - Italiano",
                "Definiciones Cuartos - Frances",
                "Definiciones Cuartos - Español",
                "Definiciones Cuartos - Portugues",
                "Definiciones Cuartos - Ruso Metro",
                "Definiciones Cuartos - Ruso",
                "Definiciones Cuartos - Libre",
                "Definiciones Cuartos - Chino Hex",
                "Definiciones Cuartos - Chino"
            ];
        }

        private void Button_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            Atributos.Add(new ReglaAtributo());
        }
    }
}
