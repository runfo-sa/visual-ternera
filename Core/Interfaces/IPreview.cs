using Core.Services;

namespace Core.Interfaces
{
    public interface IPreview
    {
        /// <summary>
        /// Contenido procesado para generar la preview
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Lista de errores encontrados durante el proceso
        /// </summary>
        public string Error { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="codigo">Codigo del producto</param>
        /// <returns>A si mismo, para concatenar metodos</returns>
        public Task<string[]?> Linting(string codigo, string dpi, string size);

        /// <summary>
        /// Completa las variables de una etiqueta con los datos de un producto especificado.
        /// </summary>
        /// <param name="codigo">Codigo del producto</param>
        /// <returns>A si mismo, para concatenar metodos</returns>
        public IPreview FillProduct(SettingsService settings, string codigo);

        /// <summary>
        /// Completa las variables de una etiqueta con datos de prueba.
        /// </summary>
        /// <returns>A si mismo, para concatenar metodos</returns>
        public IPreview FillTestVariables();

        /// <summary>
        /// Carga las fuentes de texto para poder renderizar distintos alfabetos.
        /// <br/>
        /// Los parametros para cargar otra fuente son especificados en la metadata de la etiqueta.
        /// </summary>
        /// <returns>A si mismo, para concatenar metodos</returns>
        public IPreview LoadFonts();

        /// <summary>
        /// Crea la preview de la etiqueta
        /// </summary>
        public Task<byte[]?> Build(string dpi, string size);
    }
}
