using Microsoft.Extensions.Configuration;
using Models.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EKoin.Utility
{
    public class HttpRequest:IHttpRequest
    {
        #region ctor

        private readonly IHttpClientFactory httpClient;
        private readonly IConfiguration configuration;

        public HttpRequest(IHttpClientFactory _httpClient, IConfiguration _configuration)
        {
            httpClient = _httpClient;
            configuration = _configuration;
        }

        #endregion

        public async Task<string> SubmitTransaction(SubmitTransaction submitTransaction,string hostAddress)
        {
            try
            {
                return await PostData(submitTransaction, ($"{hostAddress}/Network/ReciveTransaction"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public async Task<string> TestGenNemonic(string content, string hostAddress)
        //{
        //    StringContent stringContent= new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        //    return await PostData(stringContent, ($"{hostAddress}/Wallet/Genrate"));
        //}

        public async Task<string> PostData(StringContent content, string hostAddress)
        {
            try
            {
                HttpClient client = httpClient.CreateClient();
                //2 minute timeout on wait for response
                client.Timeout = new TimeSpan(0, 1, 0);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, hostAddress);

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", configuration["UserAgent"]);

                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);
                //if (response.IsSuccessStatusCode){}
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> PostData<T>(T t,string hostAddress)
        {
            ///test1
            try
            {
                HttpClient client = httpClient.CreateClient();
                //timeout on wait for response
                client.Timeout = new TimeSpan(0, 1, 0);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, hostAddress);

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", configuration["UserAgent"]);

                string serializedStr = JsonSerializer.Serialize(t);
                request.Content = new StringContent(serializedStr, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);
                //if (response.IsSuccessStatusCode){}
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }

            ///test3
            //HttpClient client = httpClient.CreateClient();
            //client.DefaultRequestHeaders.Add("User-Agent", configuration["UserAgent"]);
            //client.BaseAddress = new Uri("");
            //var response = await client.GetFromJsonAsync < "" > ($"users/{t}");
            ///test2
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://127.0.0.1:45997/Public/PubKx");
            //request.Headers.Add("Accept", "application/json");
            //request.Headers.Add("User-Agent", configuration["UserAgent"]);
            //HttpClient client = httpClient.CreateClient();
            //HttpResponseMessage response = await client.SendAsync(request);


        }

    }
}
