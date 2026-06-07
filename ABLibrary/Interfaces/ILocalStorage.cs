using System;

namespace ABLibrary.Interfaces
{
    public interface ILocalStorage
    {
        /// <summary> Сохранение данных </summary>
        void Save<T>(string key, T data);

        /// <summary> Загрузка данных </summary>
        T Load<T>(string key);

        /// <summary> Проверка существования файла </summary>
        bool Exists(string key);

        /// <summary> Удаление файла </summary>
        void Delete(string key);
    }
}