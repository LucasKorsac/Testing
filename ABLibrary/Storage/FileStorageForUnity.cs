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

            // Создаем директорию при инициализации, если её нет
            EnsureDirectoryExists(_basePath);
        }

        /// <summary>
        /// Сохранение данных в JSON файл
        /// </summary>
        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            var path = GetPath(key);

            // Создаем директорию для файла, если её нет
            EnsureDirectoryExists(Path.GetDirectoryName(path));

            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save data to {path}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Загрузка данных из JSON файла
        /// </summary>
        public T Load<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T);

            var path = GetPath(key);

            if (!File.Exists(path))
                return default(T);

            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to load data from {path}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Проверка существования файла
        /// </summary>
        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var path = GetPath(key);
            return File.Exists(path);
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            var path = GetPath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Получение полного пути к файлу
        /// </summary>
        private string GetPath(string key)
        {
            // Очищаем ключ от недопустимых символов
            var safeKey = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_basePath, safeKey + ".json");
        }

        /// <summary>
        /// Создание директории, если её нет
        /// </summary>
        private void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}