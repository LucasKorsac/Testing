using ABLibrary.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ABLibraryUnity.Unity
{
    public sealed class UnityFileStorage : ILocalStorage
    {
        private readonly string _basePath;

        public UnityFileStorage(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
                throw new ArgumentException(
                    "Base path cannot be empty.",
                    nameof(basePath));

            _basePath = basePath;

            Directory.CreateDirectory(_basePath);
        }

        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(
                    "Storage key cannot be empty.",
                    nameof(key));

            var path = GetPath(key);
            var tempPath = path + ".tmp";

            var json = JsonConvert.SerializeObject(
                data,
                Formatting.None);

            // Сначала пишем временный файл.
            File.WriteAllText(
                tempPath,
                json);

            // Затем заменяем основной.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(
                tempPath,
                path);
        }

        public T Load<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(
                    "Storage key cannot be empty.",
                    nameof(key));

            var path = GetPath(key);

            if (!File.Exists(path))
                return default(T);

            try
            {
                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                    return default(T);

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonException)
            {
                // Повреждённый cache не должен ломать приложение.
                return default(T);
            }
            catch (IOException)
            {
                return default(T);
            }
        }

        public bool Exists(string key)
        {
            return File.Exists(GetPath(key));
        }

        public void Delete(string key)
        {
            var path = GetPath(key);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private string GetPath(string key)
        {
            var safeKey = SanitizeKey(key);

            return Path.Combine(
                _basePath,
                safeKey + ".json");
        }

        private static string SanitizeKey(string key)
        {
            var safeKey = key;

            foreach (var character
                in Path.GetInvalidFileNameChars())
            {
                safeKey = safeKey.Replace(
                    character,
                    '_');
            }

            return safeKey;
        }
    }
}