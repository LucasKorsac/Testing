//using ABLibrary.Interfaces;
//using ABLibrary.Models;
//using Newtonsoft.Json;
//using System;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

//namespace ABLibrary.Transport
//{
//    public class HttpABTransport : IABTransport
//    {
//        private readonly string _baseUrl;

//        public HttpABTransport(string baseUrl)
//        {
//            _baseUrl = baseUrl;
//        }

//        public async Task<ServerConfig> GetConfigAsync(string appId)
//        {
//            var request = WebRequest.Create(_baseUrl + "api/ab/config/" + appId);
//            request.Method = "GET";
//            request.Timeout = 10000;

//            try
//            {
//                using (var webResponse = await request.GetResponseAsync())
//                {
//                    string json;
//                    using (var stream = webResponse.GetResponseStream())
//                    using (var reader = new StreamReader(stream))
//                    {
//                        json = await reader.ReadToEndAsync();
//                    }

//                    var result = JsonConvert.DeserializeObject<ServerConfig>(json);
//                    return result ?? new ServerConfig();
//                }
//            }
//            catch (WebException ex)
//            {
//                // В случае ошибки возвращаем пустую конфигурацию
//                Console.WriteLine($"Error getting config: {ex.Message}");
//                return new ServerConfig();
//            }
//        }

//        public async Task SendEventAsync(TestEvent evt)
//        {
//            var json = JsonConvert.SerializeObject(evt);
//            var request = WebRequest.Create(_baseUrl + "api/ab/event");
//            request.Method = "POST";
//            request.ContentType = "application/json";
//            request.Timeout = 10000;

//            byte[] bytes = Encoding.UTF8.GetBytes(json);
//            request.ContentLength = bytes.Length;

//            using (var stream = await request.GetRequestStreamAsync())
//            {
//                await stream.WriteAsync(bytes, 0, bytes.Length);
//            }

//            try
//            {
//                using (var response = await request.GetResponseAsync())
//                {
//                    // Успешная отправка
//                }
//            }
//            catch (WebException ex)
//            {
//                // Логируем ошибку, но не выбрасываем исключение
//                Console.WriteLine($"Error sending event: {ex.Message}");
//            }
//        }
//    }
//}

using ABLibrary.Interfaces;
using ABLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ABLibrary.Transport
{
    public class HttpABTransport : IABTransport, IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;

        public HttpABTransport(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public async Task<ServerConfig> GetConfigAsync(string appId)
        {
            try
            {
                // POST запрос к api/ab/run
                var runRequest = new { AppId = appId };
                var json = JsonConvert.SerializeObject(runRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/ab/run", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var tests = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);

                    return new ServerConfig
                    {
                        Tests = tests ?? new Dictionary<string, string>()
                    };
                }

                return new ServerConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting config: {ex.Message}");
                return new ServerConfig();
            }
        }

        public async Task SendEventAsync(TestEvent evt)
        {
            try
            {
                var json = JsonConvert.SerializeObject(evt);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/ab/event", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to send event: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending event: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}