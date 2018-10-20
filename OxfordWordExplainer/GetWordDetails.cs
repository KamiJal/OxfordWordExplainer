
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using OxfordWordExplainer;

namespace OxfordWordDefinition
{
    public static class GetWordDetails
    {
        private static readonly Uri _baseAddress = new Uri("https://od-api.oxforddictionaries.com/api/v1/entries/en/");
        private static readonly string _appId = "15e40921";
        private static readonly string _appKey = "9df2b408307aa00c352472f5ffc9c863";

        [FunctionName("GetWordDetails")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            string word = req.Query["word"];

            Report report = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = _baseAddress;
                httpClient.DefaultRequestHeaders.Add("app_id", _appId);
                httpClient.DefaultRequestHeaders.Add("app_key", _appKey);

                try
                {
                    var deserialized = JsonConvert.DeserializeObject<RootObject>(await httpClient.GetStringAsync(word)); ;
                    report = new Report(deserialized.results.First());
                }
                catch (HttpRequestException ex)
                {
                    report = new Report(word, ex.Message);
                }                
            }

            return GenerateHttpResponseMessage(report);
        }


        private static HttpResponseMessage GenerateHttpResponseMessage(Report report)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(report.GenerateHtmlString())
            };

            message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/html");
            return message;
        }

    }
}
