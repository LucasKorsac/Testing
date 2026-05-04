using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    /// <summary> Централизованная обработка исключений </summary>
    public static class ErrorCheck
    {
        /// <summary> Обработка исключения </summary>
        public static void Handle(Exception ex, string context = "")
        {
            var message = $"[ERROR] {DateTime.Now:HH:mm:ss}";

            if (!string.IsNullOrEmpty(context))
                message += $" | {context}";

            message += $" | {ex.GetType().Name}: {ex.Message}";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            // Можно добавить логирование в файл
            LogToFile(message);
        }

        /// <summary> Безопасное выполнение async метода </summary>
        public static async Task SafeExecuteAsync(Func<Task> action, string context = "")
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Handle(ex, context);
            }
        }

        /// <summary> Безопасное выполнение с результатом </summary>
        public static async Task<T?> SafeExecuteAsync<T>(Func<Task<T>> action, string context = "")
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                Handle(ex, context);
                return default;
            }
        }

        /// <summary> Логирование в файл </summary>
        private static void LogToFile(string message)
        {
            try
            {
                File.AppendAllText("errors.log", message + Environment.NewLine);
            }
            catch
            {
                // если файл не записался — игнор
            }
        }
    
    }
}
