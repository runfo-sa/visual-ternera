using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Editor.Model
{
    public class Labelary
    {
        private readonly HttpClient _client = new();

        public async Task<byte[]?> Post(string content, string dpi, string size)
        {
            try
            {
                StringContent jsonBody = new(
                    JsonSerializer.Serialize(content),
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
