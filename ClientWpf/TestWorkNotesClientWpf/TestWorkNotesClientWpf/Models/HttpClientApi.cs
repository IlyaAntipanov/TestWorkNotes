using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestWorkNotesClientWpf.Models
{
    public static class HttpClientApi
    {
        private static HttpClient HttpClient;
        public static event Action<string> LogEvent;
        static HttpClientApi()
        {
            HttpClient = new HttpClient();
        }

        public static async Task<T> GetAsync<T>(string path, Dictionary<string, string> query = null)
        {
            var result = await SendAsync(path, HttpMethod.Get, null, query, null);
            if (!result.IsSuccessStatusCode)
            {
                OnLogEvent($"Error : {result.StatusCode}");
                return default(T);
            }
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<HttpResponseMessage> PostAsync(string path, HttpContent form = null,
            Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            return await SendAsync(path, HttpMethod.Post, form, query, headers);
        }

        public static async Task<T> PostAsync<T>(string path, HttpContent form = null,
            Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            var result = await SendAsync(path, HttpMethod.Post, form, query, headers);
            if (!result.IsSuccessStatusCode)
            {
                OnLogEvent($"Error : {result.StatusCode}");
                return default(T);
            }
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<int> PostAsync(string path, string form)
        {
            OnLogEvent($"POST : {path}");
            var content = new StringContent($"\"{form}\"", Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(path, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                OnLogEvent($"Error: {errorContent}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return int.Parse(responseContent);
        }

        public static async Task<HttpResponseMessage> PutAsync(string path, HttpContent form = null,
            Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            return await SendAsync(path, HttpMethod.Put, form, query, headers);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string path,
            Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            return await SendAsync(path, HttpMethod.Delete, null, query, headers);
        }

        private static void OnLogEvent(string text)
        {
            LogEvent?.Invoke(text);
        }

        private static async Task<HttpResponseMessage> SendAsync(string path, HttpMethod httpMethod, HttpContent form = null,
            Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            var newPath = path;
            if (query != null)
            {
                newPath += new StringBuilder("?").Append(query.Select(s => new StringBuilder($"{s.Key}={s.Value}")).Aggregate((a, b) => a.Append('&').Append(b))).ToString();
            }
            var request = new HttpRequestMessage(httpMethod, newPath);
            if (form != null)
                request.Content = form;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            OnLogEvent($"{httpMethod} : {newPath}");
          return await HttpClient.SendAsync(request);
        }
    }
}
