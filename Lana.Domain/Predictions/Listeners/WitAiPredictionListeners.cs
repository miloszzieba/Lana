using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lana.Core.Interfaces;
using Newtonsoft.Json;

namespace Lana.Domain.Predictions.Listeners
{
    public class WitAiPredictionListener : IPredictionListener
    {
        private readonly string _witAIAuthorizationToken;

        public WitAiPredictionListener(string witAIAuthorizationToken)
        {
            this._witAIAuthorizationToken = witAIAuthorizationToken;
        }

        public string Name { get; } = "Wit.AI";

        public string Listen(MemoryStream audioStream)
        {
            if (audioStream == null)
                throw new ArgumentNullException(nameof(audioStream));

            var byteArray = audioStream.ToArray();
            return PostAudio(audioStream).GetAwaiter().GetResult();
        }

        private async Task<string> PostAudio(MemoryStream audioStream)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + _witAIAuthorizationToken);

                using (var content = new ByteArrayContent(audioStream.ToArray()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
                    var uri = new Uri("https://api.wit.ai/speech");
                    var message = await client.PostAsync(uri, content).ConfigureAwait(false);

                    var info = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var obj = JsonConvert.DeserializeObject<RootObject>(info);

                    return obj._text;
                }
            }
        }
#pragma warning disable CA1812
        private class RootObject
#pragma warning restore CA1812
        {
#pragma warning disable IDE1006 // Naming Styles
            public string _text { get; set; }
            public object entities { get; set; }
            public string msg_id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}
