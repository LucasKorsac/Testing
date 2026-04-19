using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Testing
{
    internal class FilesWork
    {
        /// <summary>
        /// Работа с файлами: TXT и XML экспорт/импорт
        /// </summary>
        public class DataFileService
        {
            // TXT

            /// <summary>
            /// Запись строк в TXT файл
            /// </summary>
            public void WriteTxt(string path, IEnumerable<string> lines)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                File.WriteAllLines(path, lines, Encoding.UTF8);
            }

            /// <summary>
            /// Добавление строки в TXT файл
            /// </summary>
            public void AppendTxt(string path, string line)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
            }

            /// <summary>
            /// Чтение TXT файла
            /// </summary>
            public List<string> ReadTxt(string path)
            {
                if (!File.Exists(path))
                    return new List<string>();

                return new List<string>(File.ReadAllLines(path, Encoding.UTF8));
            }

            // XML

            /// <summary>
            /// Запись простых ключ-значение данных в XML
            /// </summary>
            public void WriteXml(string path, Dictionary<string, string> data, string rootName = "Root")
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                var root = new XElement(rootName);

                foreach (var item in data)
                {
                    root.Add(new XElement(item.Key, item.Value));
                }

                var doc = new XDocument(root);
                doc.Save(path);
            }

            /// <summary>
            /// Запись списка объектов в XML
            /// </summary>
            public void WriteXmlList<T>(string path, IEnumerable<T> items, string rootName = "Items", string itemName = "Item")
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                var root = new XElement(rootName);

                foreach (var item in items)
                {
                    var element = new XElement(itemName);

                    foreach (var prop in typeof(T).GetProperties())
                    {
                        var value = prop.GetValue(item)?.ToString() ?? "";
                        element.Add(new XElement(prop.Name, value));
                    }

                    root.Add(element);
                }

                var doc = new XDocument(root);
                doc.Save(path);
            }

            /// <summary>
            /// Чтение XML
            /// </summary>
            public Dictionary<string, string> ReadXml(string path)
            {
                var result = new Dictionary<string, string>();

                if (!File.Exists(path))
                    return result;

                var doc = XDocument.Load(path);

                foreach (var element in doc.Root!.Elements())
                {
                    result[element.Name.LocalName] = element.Value;
                }

                return result;
            }
        }
    }
}
