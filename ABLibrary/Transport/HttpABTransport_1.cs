
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

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        //public async Task<ServerConfig> GetConfigAsync(string appId)
        //{
        //    try
        //    {
        //        // POST запрос к api/ab/run
        //        var runRequest = new { AppId = appId };
        //        var json = JsonConvert.SerializeObject(runRequest);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = await _httpClient.PostAsync("api/ab/run", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var resultJson = await response.Content.ReadAsStringAsync();
        //            var tests = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);

        //            return new ServerConfig
        //            {
        //                Tests = tests ?? new Dictionary<string, string>()
        //            };
        //        }

        //        return new ServerConfig();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error getting config: {ex.Message}");
        //        return new ServerConfig();
        //    }
        //}



        //        public async Task SendEventAsync(TestEvent evt)
        //        {
        //            try
        //            {
        //                var json = JsonConvert.SerializeObject(evt);
        //                var content = new StringContent(json, Encoding.UTF8, "application/json");

        //                var response = await _httpClient.PostAsync("api/ab/event", content);

        //                if (!response.IsSuccessStatusCode)
        //                {
        //                    Console.WriteLine($"Failed to send event: {response.StatusCode}");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error sending event: {ex.Message}");
        //            }
        //        }

        //        public void Dispose()
        //        {
        //            if (!_disposed)
        //            {
        //                _httpClient?.Dispose();
        //                _disposed = true;
        //            }
        //        }
        //    }
        //}

        public async Task<ServerConfig> GetConfigAsync(string appId, string instanceId = "")
        {
            try
            {
                var request = new { AppId = appId, InstanceId = instanceId };
                var json = JsonConvert.SerializeObject(request);
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