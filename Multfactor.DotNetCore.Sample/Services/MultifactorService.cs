using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Multfactor.DotNetCore.Sample.Services
{
    /// <summary>
    /// Multifactor Client
    /// </summary>
    public class MultifactorService
    {
        private string _apiKey;
        private string _apiSecret;
        private string _callbackUrl;

        private string _apiHost = "https://api.multifactor.ru";

        public MultifactorService(string apiKey, string apiSecret, string callbackUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
            _callbackUrl = callbackUrl ?? throw new ArgumentNullException(nameof(callbackUrl));
        }

        public async Task<string> GetAccessPage(string identityName, IDictionary<string, string> claims = null)
        {
            if (string.IsNullOrEmpty(identityName)) throw new ArgumentNullException(nameof(identityName));

            //api implementation
            //see https://multifactor.ru/docs/api/

            var request = JsonConvert.SerializeObject(new
            {
                Identity = identityName,
                Callback = new
                {
                    Action = _callbackUrl,
                    Target = "_self"
                },
                Claims = claims
            });

            var payLoad = Encoding.UTF8.GetBytes(request);

            //basic authorization
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiKey + ":" + _apiSecret));
            
            using var client = new WebClient();
            client.Headers.Add("Authorization", "Basic " + authHeader);
            client.Headers.Add("Content-Type", "application/json");

            var responseData = await client.UploadDataTaskAsync(_apiHost + "/access/requests", "POST", payLoad);
            var responseJson = Encoding.ASCII.GetString(responseData);

            var response = JsonConvert.DeserializeObject<MultifactorResponse<MultifactorAccessPage>>(responseJson);

            return response.Model.Url;
        }
   
        internal class MultifactorResponse<T>
        {
            public bool Success { get; set; }

            public T Model { get; set; }
        }

        internal class MultifactorAccessPage
        {
            public string Url { get; set; }
        }
    }

}
