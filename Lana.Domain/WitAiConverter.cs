using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lana.Domain
{
    public class WitAiConverter
    {
        public string Token { get; set; } = ConfigurationManager.AppSettings["WitAiToken"];
        public Exception Exception { get; protected set; }

        public async Task<string> Convert(byte[] bytes)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + Token);

                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

                var message = await client.PostAsync("https://api.wit.ai/speech", content);

                var info = await message.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<RootObject>(info);

                return obj._text;
            }
        }

        private class RootObject
        {
            public string _text { get; set; }
            public object entities { get; set; }
            public string msg_id { get; set; }
        }
    }
}
