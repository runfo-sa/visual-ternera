using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Editor.Model
{
    public class Labelary(string content)
    {
        private record struct Font(char Name, string Fontname);

        private readonly HttpClient _client = new();

        private string _content = content;

        public Labelary FillVariables()
        {
            int startIdx;
            int endIdx = _content.LastIndexOf("@]");

            while (endIdx > 0)
            {
                startIdx = _content.LastIndexOf("[@", endIdx);
                if (startIdx > 0)
                {
                    Trace.WriteLine(_content[(startIdx + 2)..endIdx]);
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
                StringContent jsonBody = new(
                    JsonSerializer.Serialize(_content),
                    Encoding.ASCII,
                    "application/x-www-form-urlencoded"
                );

                string uri = $"http://api.labelary.com/v1/printers/{dpi}dpmm/labels/{size}/0/";
                HttpResponseMessage response = await _client.PostAsync(uri, jsonBody);
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
