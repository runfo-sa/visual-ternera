using Core.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Core.ViewLogic
{
    /// <summary>
    /// Genera el arbol de directorios, utilizar la propiedad Root para desplegar el arbol.
    /// </summary>
    public class TreeGenerator(SettingsService settings)
    {
        /// <summary>
        /// Raiz del arbol de directorios.
        /// </summary>
        public ObservableCollection<object> Root => InitTree();

        private readonly SettingsService _settings = settings;
        private ObservableCollection<object>? _cachedRoot;

        private ObservableCollection<object> InitTree()
        {
            if (_cachedRoot != null)
            {
                return _cachedRoot;
            }

            var files = Directory.GetFiles(_settings.EtiquetasDir, $"*.{_settings.EtiquetasExtension}");
            List<VirtualDirectory> dirs = [new VirtualDirectory("Otros")];
            _cachedRoot = [];

            foreach (var dir in _settings.VirtualDirectories)
            {
                dirs.Add(new VirtualDirectory(dir.Name));
                Trace.WriteLine(dir);
            }

            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                foreach (var dir in _settings.VirtualDirectories)
                {
                    if (filename.Contains(dir.Filter, StringComparison.CurrentCultureIgnoreCase))
                    {
                        dirs.First(d => d.Name == dir.Name).Files.Add(new LabelFile(file));
                        goto OuterLoop; // Despues de agregar el archivo al directorio virtual saltamos al final del loop.
                    }
                }

                // Si no se pudo agregar el archivo a ningun directorio virtual definido, se lo asigna al directorio general "Otros".
                dirs[0].Files.Add(new LabelFile(file));

            OuterLoop:
                continue;
            }

            var sortedDirs = dirs.OrderBy(d => d.Name);
            foreach (var dir in sortedDirs)
            {
                _cachedRoot.Add(dir);
            }

            return _cachedRoot;
        }
    }
}
