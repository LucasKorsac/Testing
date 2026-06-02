using ABLibrary.Interfaces;
using ABLibrary.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ABLibrary.Transport
{
    public class HttpABTransport : IABTransport
    {
        private readonly string _baseUrl;

        // НОВЫЙ КОНСТРУКТОР: принимает строку с базовым URL
        public HttpABTransport(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        // СТАРЫЙ КОНСТРУКТОР (закомментирован, использовал HttpClient)
        //private readonly HttpClient _http;
        //public HttpABTransport(HttpClient http)
        //{
        //    _http = http;
        //}

        public async Task<ServerConfig> GetConfigAsync(string appId)
        {
            // СТАРЫЙ КОД (использовал HttpClient)
            //var response = await _http.GetAsync("api/ab/config/" + appId);

            // НОВЫЙ КОД: используем WebRequest вместо HttpClient
            // Создаем запрос к серверу
            var request = WebRequest.Create(_baseUrl + "api/ab/config/" + appId);
            request.Method = "GET";  // Устанавливаем метод GET
            request.Timeout = 10000; // Таймаут 10 секунд

            // Получаем ответ от сервера
            var webResponse = await request.GetResponseAsync();

            // Читаем JSON из ответа
            string json;
            using (var stream = webResponse.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            // СТАРЫЙ КОД (для HttpClient)
            //response.EnsureSuccessStatusCode();
            //var json = await response.Content.ReadAsStringAsync();

            // Десериализуем JSON в объект
            var result = JsonConvert.DeserializeObject<ServerConfig>(json);

            return result ?? new ServerConfig();
        }

        public async Task SendEventAsync(TestEvent evt)
        {
            // Сериализуем событие в JSON
            var json = JsonConvert.SerializeObject(evt);

            // НОВЫЙ КОД: создаем и отправляем POST запрос через WebRequest
            var request = WebRequest.Create(_baseUrl + "api/ab/event");
            request.Method = "POST";           // Метод POST
            request.ContentType = "application/json"; // Тип содержимого
            request.Timeout = 10000;           // Таймаут 10 секунд

            // Преобразуем JSON в байты для отправки
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;

            // Отправляем данные
            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

            // Получаем ответ от сервера
            using (var response = await request.GetResponseAsync())
            {
                // Успешная отправка, ничего не делаем
            }

            // СТАРЫЙ КОД (использовал HttpClient)
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //var response = await _http.PostAsync("api/ab/event", content);
            //response.EnsureSuccessStatusCode();
        }
    }
}