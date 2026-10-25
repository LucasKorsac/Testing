//using System;
//using System.IO;
//using System.Text.Json;
//using ABLibrary.Interfaces;

//namespace ABLibrary.Storage
//{

//    public class FileStorage : ILocalStorage
//    {
//        private readonly string _basePath;

//        public FileStorage(string? basePath = null)
//        {
//            _basePath = basePath ?? AppContext.BaseDirectory;
//        }

//        public void Save<T>(string key, T data)
//        {
//            var path = GetPath(key);

//            File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
//        }

//        public T Load<T>(string key)
//        {
//            var path = GetPath(key);

//            if (!File.Exists(path))
//                return default;

//            var json = File.ReadAllText(path);

//            return JsonSerializer.Deserialize<T>(json);
//        }

//        private string GetPath(string key)
//        {
//            return Path.Combine(_basePath, $"{key}.json");
//        }
//    }
//}