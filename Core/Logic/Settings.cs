using YamlDotNet.Serialization;

namespace Core.Logic
{
    public class Settings
    {
        [YamlMember(Description = " - Dirección de etiquetas")]
        public required string EtiquetasDir { get; set; }

        [YamlMember(Description = " - Cadena de conexión con la base de datos SQL Server")]
        public required string SqlConnection { get; set; }

        [YamlMember(Description = " - Dirección al archivo template de RE-CAL-22")]
        public string? RecallTemplate { get; set; }

        [YamlMember(Description = " - Tema del editor, posibles modos: dark, light")]
        public Theme Theme { get; set; } = Theme.Dark;

        [YamlMember(Description = " - Region a usar por el programa, para darle formato a las fechas y numeros")]
        public required string Culture { get; set; } = "es-MX";

        [YamlMember(Description = " - Repositorio donde se almacenan globalmente las eitquetas")]
        public required string GitRepo { get; set; }

        [YamlMember(Description = " - Extensión de los archivos de etiqueta, sirve para filtrar")]
        public required string EtiquetasExtension { get; set; } = "e01";
    }

    public enum Theme
    {
        Dark,
        Light
    }
}
