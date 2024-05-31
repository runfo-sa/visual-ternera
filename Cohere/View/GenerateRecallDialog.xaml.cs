using AdonisUI.Controls;
using Core.Logic;
using Core.MVVM;
using Microsoft.Office.Interop.Word;
using System.IO;
using static Cohere.View.GenerateSample;
using Range = Microsoft.Office.Interop.Word.Range;

namespace Cohere.View
{
    public partial class GenerateRecallDialog : AdonisWindow
    {
        private readonly Settings _settings;
        private readonly IEnumerable<ProductoMuestra> _products;

        public RelayCommand AceptDialog => new(_ =>
        {
            var wordApp = new Application();

            var doc = wordApp.Documents.Open(_settings.RecallTemplate);

            SetBookmark(doc, "destino", destino.Text);
            SetBookmark(doc, "autor", autor.Text);
            SetBookmark(doc, "detalle", detalle.Text);
            SetBookmark(doc, "etiqueta_after", etiqueta_after.Text);
            SetBookmark(doc, "etiqueta_before", etiqueta_before.Text);
            SetBookmark(doc, "fecha_solicitud", fecha_solicitud.Text);
            SetBookmark(doc, "impresora", impresora.Text);
            SetBookmark(doc, "motivo", motivo.Text);
            SetBookmark(doc, "observaciones", observaciones.Text);
            SetBookmark(doc, "solicitado", solicitado.Text);

            foreach (var prod in _products)
            {
                Row row = doc.Tables[2].Rows.Add();
                row.Cells[1].Range.Text = prod.Senasa;
                row.Cells[1].Range.Font.Bold = 0;
                row.Cells[2].Range.Text = prod.Nombre;
                row.Cells[2].Range.Font.Bold = 0;
                row.Cells[3].Range.Text = prod.Codigo;
                row.Cells[3].Range.Font.Bold = 0;
            }

            var timestamp = DateTime.Now.ToString("yyMMdd_HH-mm-ss");
            doc.SaveAs(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"RE-CAL-22_{timestamp}.docx"));
            doc.Close();
            wordApp.Quit();

            DialogResult = true;
        });

        public GenerateRecallDialog(Settings settings, IEnumerable<ProductoMuestra> products)
        {
            InitializeComponent();
            DataContext = this;
            _settings = settings;
            _products = products;
        }

        private static void SetBookmark(Document doc, string bookmark, string value)
        {
            Bookmark bkm = doc.Bookmarks[bookmark];
            Range range = bkm.Range;
            range.Text = value;
        }
    }
}
