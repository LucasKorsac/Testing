using ABLibrary.Interfaces;
using System.Collections.Generic;

namespace ABLibrary.Storage
{
    public class MemoryStorage : ILocalStorage
    {
        private readonly Dictionary<string, object> _data =
            new Dictionary<string, object>();

        public void Save<T>(
            string key,
            T data)
        {
            _data[key] = data;
        }

        public T Load<T>(string key)
        {
            if (!_data.TryGetValue(
                    key,
                    out var value))
            {
                return default(T);
            }

            return value is T typed
                ? typed
                : default(T);
        }

        public bool Exists(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Delete(string key)
        {
            _data.Remove(key);
        }
    }
}