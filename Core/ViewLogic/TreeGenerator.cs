using System.Collections.ObjectModel;
using System.IO;
using Core.Logic;

namespace Core.ViewLogic
{
    public class TreeGenerator(Settings settings)
    {
        private readonly Settings _settings = settings;
        public ObservableCollection<object> Root => InitTree();

        private ObservableCollection<object> InitTree()
        {
            ObservableCollection<object> root = [];

            var caja = new LabelDir("Caja");
            var otro = new LabelDir("Otros");
            var prim = new LabelDir("Primaria");

            var files = Directory.GetFiles(_settings.EtiquetasDir, $"*.{_settings.EtiquetasExtension}");
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                if (filename.StartsWith("CAJA", StringComparison.CurrentCultureIgnoreCase))
                {
                    caja.Labels.Add(new LabelFile(file));
                }
                else if (filename.StartsWith("PRIMARIA", StringComparison.CurrentCultureIgnoreCase))
                {
                    prim.Labels.Add(new LabelFile(file));
                }
                else
                {
                    otro.Labels.Add(new LabelFile(file));
                }
            }

            root.Add(caja);
            root.Add(prim);
            root.Add(otro);

            return root;
        }
    }
}
