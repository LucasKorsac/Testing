using ABLibrary.Interfaces;
using ABLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABLibrary.Transport
{
    public class HttpABTransport : IABTransport, IDisposable
    {
        private readonly HttpClient _httpClient;

        private bool _disposed;

        public HttpABTransport(
            string baseUrl,
            TimeSpan? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException(
                    "Base URL cannot be empty.",
                    nameof(baseUrl));

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = timeout ??
                          TimeSpan.FromSeconds(10)
            };

            _httpClient
                .DefaultRequestHeaders
                .TryAddWithoutValidation(
                    "Accept",
                    "application/json");
        }

        public async Task<ServerConfig> GetConfigAsync(
            string applicationId,
            string userId,
            string instanceId,
            CancellationToken cancellationToken = default)
        {
            var request = new
            {
                ApplicationId = applicationId,
                UserId = userId,
                InstanceId = instanceId
            };

            var json =
                JsonConvert.SerializeObject(request);

            using (var content =
                   new StringContent(
                       json,
                       Encoding.UTF8,
                       "application/json"))
            {
                using (var response =
                       await _httpClient.PostAsync(
                           "api/ab/assign",
                           content,
                           cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(
                            "AB configuration request failed. " +
                            "Status: " +
                            response.StatusCode);
                    }

                    var responseJson =
                        await response.Content
                            .ReadAsStringAsync();

                    var config =
                        JsonConvert
                            .DeserializeObject<ServerConfig>(
                                responseJson);

                    return config
                           ?? new ServerConfig();
                }
            }
        }

        public async Task<EventBatchResponse> SendEventsAsync(
            IReadOnlyCollection<TestEvent> events,
            CancellationToken cancellationToken = default)
        {
            if (events == null || events.Count == 0)
            {
                return new EventBatchResponse
                {
                    Success = true,
                    Accepted = 0,
                    Failed = 0
                };
            }

            var json =
                JsonConvert.SerializeObject(events);

            using (var content =
                   new StringContent(
                       json,
                       Encoding.UTF8,
                       "application/json"))
            {
                using (var response =
                       await _httpClient.PostAsync(
                           "api/ab/events",
                           content,
                           cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return new EventBatchResponse
                        {
                            Success = false,
                            Accepted = 0,
                            Failed = events.Count,
                            Error =
                                "HTTP " +
                                response.StatusCode
                        };
                    }

                    var responseJson =
                        await response.Content
                            .ReadAsStringAsync();

                    var result =
                        JsonConvert
                            .DeserializeObject<EventBatchResponse>(
                                responseJson);

                    return result ??
                        new EventBatchResponse
                         {
                            Success = true,
                            Accepted = events.Count,
                            Failed = 0
                         };
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _httpClient.Dispose();

            _disposed = true;
        }
    }
}