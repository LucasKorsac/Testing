using System;

namespace Testing.Pattern
{
    /// <summary> Интерфейс логирования репозитория </summary>
    public interface IRepositoryLogger
    {
        /// <summary> Информационное сообщение </summary>
        void Log(string message);

        /// <summary> Сообщение об ошибке </summary>
        void Error(string message);
    }

    /// <summary> Консольный логгер репозиториев MongoDB </summary>
    public class RepositoryLogger : IRepositoryLogger
    {
        /// <summary> Информационный лог </summary>
        public void Log(string message)
        {
            Write("LOG", message);
        }

        /// <summary> Лог ошибки </summary>
        public void Error(string message)
        {
            Write("ERROR", message);
        }

        /// <summary> Вывод сообщения </summary>
        private void Write(string level, string message)
        {
            Console.WriteLine($"[{level}] {DateTime.Now:HH:mm:ss} | {message}"
            );
        }
    }
}