namespace Core.Logic
{
    /// <summary>
    /// Metadata para una etiqueta,
    /// util para tener variables de como se generaria la muestra
    /// o para integrar con alguna funcionalidad del IDE.
    /// </summary>
    public class LabelMetadata<LanguageImpl>
    {
        /// <summary>
        /// Lista de lenguajes con alfabetos no encontrados en la tabla ASCII.
        /// </summary>
        public List<LanguageImpl>? Languages { get; set; }
    }
}
