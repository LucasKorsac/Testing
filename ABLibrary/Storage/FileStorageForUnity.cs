using System;
using System.IO;
using Newtonsoft.Json;
using ABLibrary.Interfaces;

namespace ABLibrary.Storage
{
   public class FileStorageForUnity : ILocalStorage
    {
        private readonly string _basePath;

        public FileStorageForUnity(string basePath = null)
        {
            _basePath = basePath ?? AppContext.BaseDirectory;
        }

        public void Save<T>(string key, T data)
        {
            var path = GetPath(key);

            var json = JsonConvert.SerializeObject(
                data,
                Formatting.Indented);

            File.WriteAllText(path, json);
        }

        public T Load<T>(string key)
        {
            var path = GetPath(key);

            if (!File.Exists(path))
                return default(T);

            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(json);
        }

        private string GetPath(string key)
        {
            return Path.Combine(_basePath, key + ".json");
        }
    }
}
