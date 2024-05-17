using System.Diagnostics;
using System.Net.Http;
using System.Text;
using YamlDotNet.Serialization;

namespace Editor.Model
{
    public class Labelary(string content)
    {
        private record struct Font(char Name, string Fontname);

        private readonly HttpClient _client = new();

        private string _content = content;

        public string Content => _content;

        public Labelary FillVariables(Core.Settings settings)
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
