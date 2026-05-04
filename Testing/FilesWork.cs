using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Testing
{
    internal class FilesWork
    {
        /// <summary> Сервис для работы с файлами TXT и JSON </summary>
        public class DataFileService
        {
            /// <summary> Создаёт папку, если она не существует </summary>
            private void EnsureDirectory(string path)
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
            }

            // TXT 

            /// <summary> Запись списка строк в TXT файл </summary>
            public void WriteTxt(string path, IEnumerable<string> lines)
            {
                EnsureDirectory(path);
                File.WriteAllLines(path, lines, Encoding.UTF8);
            }

            /// <summary> Добавление одной строки в конец TXT файла </summary>
            public void AppendTxt(string path, string line)
            {
                EnsureDirectory(path);
                File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
            }

            /// <summary> Чтение всех строк из TXT файла </summary>
            public List<string> ReadTxt(string path)
            {
                return File.Exists(path)
                    ? new List<string>(File.ReadAllLines(path, Encoding.UTF8))
                    : new List<string>();
            }

            // JSON

            /// <summary> Сериализация объекта в JSON файл </summary>
            public void WriteJson<T>(string path, T data)
            {
                EnsureDirectory(path);

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(path, json, Encoding.UTF8);
            }

            /// <summary> Десериализация объекта из JSON файла </summary>
            public T? ReadJson<T>(string path)
            {
                if (!File.Exists(path))
                    return default;

                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonSerializer.Deserialize<T>(json);
            }

            ////  XML

            ///// <summary>
            ///// Запись словаря (ключ-значение) в XML файл
            ///// </summary>
            //public void WriteXml(string path, Dictionary<string, string> data, string rootName = "Root")
            //{
            //    EnsureDirectory(path);

            //    var root = new XElement(rootName,
            //        data.Select(d => new XElement(d.Key, d.Value)));

            //    new XDocument(root).Save(path);
            //}

            ///// <summary>
            ///// Запись списка объектов в XML (через рефлексию)
            ///// </summary>
            //public void WriteXmlList<T>(string path, IEnumerable<T> items, string rootName = "Items", string itemName = "Item")
            //{
            //    EnsureDirectory(path);

            //    var root = new XElement(rootName);

            //    foreach (var item in items)
            //    {
            //        var element = new XElement(itemName);

            //        foreach (var prop in typeof(T).GetProperties())
            //        {
            //            var value = prop.GetValue(item)?.ToString() ?? "";
            //            element.Add(new XElement(prop.Name, value));
            //        }

            //        root.Add(element);
            //    }

            //    new XDocument(root).Save(path);
            //}

            ///// <summary>
            ///// Чтение XML файла в словарь
            ///// </summary>
            //public Dictionary<string, string> ReadXml(string path)
            //{
            //    if (!File.Exists(path))
            //        return new Dictionary<string, string>();

            //    var doc = XDocument.Load(path);

            //    return doc.Root!
            //        .Elements()
            //        .ToDictionary(e => e.Name.LocalName, e => e.Value);
            //}
        }
    }
}