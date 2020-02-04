﻿using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lana.Domain
{
    public class WitAiParser
    {
        public string Token { get; set; } = ConfigurationManager.AppSettings["WitAiToken"];
        public Exception Exception { get; protected set; }

        public async Task<string> Parse(byte[] bytes)
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
#pragma warning disable IDE1006 // Naming Styles
            public string _text { get; set; }
            public object entities { get; set; }
            public string msg_id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}