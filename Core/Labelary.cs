using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using YamlDotNet.Serialization;

namespace Core
{
    public class Labelary(string content)
    {
        private record struct Font(char Name, string Fontname);

        private readonly HttpClient _client = new();

        private string _content = content;

        public string Content => _content;

        public Labelary FillProduct(Settings settings, string codigo)
        {
            var context = new TestVarsDbContext(settings.SqlConnection);
            var codigoParam = new SqlParameter("@Codigo", codigo);

            int startIdx;
            int endIdx = _content.LastIndexOf("@]");

            while (endIdx > 0)
            {
                startIdx = _content.LastIndexOf("[@", endIdx);
                if (startIdx > 0)
                {
                    var key = _content[(startIdx + 2)..endIdx].ToLower();
                    if (key.Contains("DefinicionesCuartos", StringComparison.CurrentCultureIgnoreCase) ||
                        key.Contains("MercaderiasTraducciones", StringComparison.CurrentCultureIgnoreCase) ||
                        key.Contains("Temperaturas", StringComparison.CurrentCultureIgnoreCase) ||
                        key.Contains("EAN", StringComparison.CurrentCultureIgnoreCase) ||
                        key.Contains("FechaVencimiento", StringComparison.CurrentCultureIgnoreCase) ||
                        key.Contains("CodSenasa", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ReplaceVar(key, codigoParam, context);
                    }
                }
                endIdx = _content.LastIndexOf("@]", startIdx);
            }
            return this;
        }

        private void ReplaceVar(string key, SqlParameter codigoParam, TestVarsDbContext context)
        {
            var argParam = new SqlParameter("@Arg", string.Concat(key.Where(Char.IsDigit)));
            if (argParam.Value.ToString().IsNullOrEmpty())
            {
                argParam.Value = key[(key.LastIndexOf(";FF", StringComparison.CurrentCultureIgnoreCase) + 3)..].Replace('m', 'M');
            }
            var varParam = new SqlParameter("@Var", key);

            var result = context.Database
                .SqlQueryRaw<ValorProductos>("ide.CompletarProducto @Codigo, @Arg, @Var", codigoParam, argParam, varParam)
                .ToList();

            if (result.Count > 0)
            {
                _content = _content.Replace($"[@{key}@]", result[0].Valor, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public Labelary FillVariables(Settings settings)
        {
            var dbContext = new TestVarsDbContext(settings.SqlConnection);
            List<TestVar> vars = [.. dbContext.EtiquetasDatosPrueba];
            Dictionary<string, string> keyValues = vars.ToDictionary(x => x.Key, v => v.Value);

            int startIdx;
            int endIdx = _content.LastIndexOf("@]");

            while (endIdx > 0)
            {
                startIdx = _content.LastIndexOf("[@", endIdx);
                if (startIdx > 0)
                {
                    var key = _content[(startIdx + 2)..endIdx].ToLower();
                    if (keyValues.TryGetValue(key, out var value))
                    {
                        _content = _content.Replace($"[@{key}@]", value, StringComparison.CurrentCultureIgnoreCase);
                    }
                    else
                    {
                        _content = _content.Replace($"[@{key}@]", "", StringComparison.CurrentCultureIgnoreCase);
                    }
                }
                endIdx = _content.LastIndexOf("@]", startIdx);
            }
            return this;
        }

        public Labelary LoadFonts()
        {
            var startIdx = _content.IndexOf("^FX Start Metadata");
            var endIdx = _content.IndexOf("^FX End Metadata");

            if (startIdx > 0 && endIdx > 0)
            {
                var rawMetadata = _content[(startIdx + "^FX Start Metadata".Length)..endIdx]
                    .Trim()
                    .Replace("^FX ", "");

                var deserializer = new DeserializerBuilder().Build();
                LabelMetadata metadata = deserializer.Deserialize<LabelMetadata>(rawMetadata);
                if (metadata.Language is not null)
                {
                    _content = metadata.Language?.ParseContent(_content)!;
                }
            }

            return this;
        }

        public async Task<byte[]?> Post(string dpi, string size)
        {
            try
            {
                StringContent body = new(
                    @_content,
                    Encoding.ASCII,
                    "application/x-www-form-urlencoded"
                );

                string uri = $"http://api.labelary.com/v1/printers/{dpi}dpmm/labels/{size}/0/";
                HttpResponseMessage response = await _client.PostAsync(uri, body);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception err)
            {
                Trace.TraceError(err.Message);
            }

            return null;
        }
    }
}
