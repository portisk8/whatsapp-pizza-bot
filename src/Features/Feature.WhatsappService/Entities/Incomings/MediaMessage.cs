using Feature.WhatsappService.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using WhatsApp.Adapters.WhatsAppAdapter.Entities;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class MediaMessage
    {

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        public async Task<byte[]> GetMedia(string url, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                //var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{url}/{Id}");
                //requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                //var response = await client.SendAsync(requestMessage);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await client.GetAsync($"{url}/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    string stringifiedBody;
                    using (var sr = new StreamReader(response.Content.ReadAsStream()))
                    {
                        stringifiedBody = await sr.ReadToEndAsync().ConfigureAwait(false);
                    }

                    var incomingMessage = JsonSerializer.Deserialize<MediaFileData>(stringifiedBody);
                    response = await client.GetAsync(incomingMessage.Url);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsByteArrayAsync();
                    }

                }
                return null;
            }
        }

        public async Task<MediaFile> GetMediaFileAsync(string url, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                //var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{url}/{Id}");
                //requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                //var response = await client.SendAsync(requestMessage);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await client.GetAsync($"{url}/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    string stringifiedBody;
                    using (var sr = new StreamReader(response.Content.ReadAsStream()))
                    {
                        stringifiedBody = await sr.ReadToEndAsync().ConfigureAwait(false);
                    }

                    var incomingMessage = JsonSerializer.Deserialize<MediaFileData>(stringifiedBody);
                    response = await client.GetAsync(incomingMessage.Url);
                    if (response.IsSuccessStatusCode)
                    {
                        var fileArray = await response.Content.ReadAsByteArrayAsync();
                        return new MediaFile
                        {
                            FileArray = fileArray,
                            ContentType = response.Content.Headers.ContentType.MediaType
                        };
                    }

                }
                return null;
            }
        }
    }
}