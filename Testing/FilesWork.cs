using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Testing
{
    internal class FilesWork
    {
        /// <summary> Сервис для работы с файлами JSON и XLSX </summary>
        public class DataFileService
        {
            private void EnsureDirectory(string path)
            {
                var dir = Path.GetDirectoryName(path);

                if (!string.IsNullOrWhiteSpace(dir))
                    Directory.CreateDirectory(dir);
            }

            // JSON

            /// <summary> Сериализация объекта в JSON файл </summary>
            public void WriteJson<T>(string path, T data)
            {
                try
                {
                    EnsureDirectory(path);

                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    File.WriteAllText(path, json, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    throw new IOException($"WriteJson failed: {path}", ex);
                }
            }

            /// <summary> Десериализация объекта из JSON файла </summary>
            public T? ReadJson<T>(string path)
            {
                try
                {
                    if (!File.Exists(path))
                        return default;

                    var json = File.ReadAllText(path, Encoding.UTF8);

                    return JsonSerializer.Deserialize<T>(json);
                }
                catch (Exception ex)
                {
                    throw new IOException($"ReadJson failed: {path}", ex);
                }
            }
        }
    }
}