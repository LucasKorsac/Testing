using System;
using System.Threading.Tasks;

namespace Testing
{
    internal class Program
    {
        static async Task Main()
        {
            // Поменять для соединения
            var app = new App();
            await app.Init();

            foreach (var kvp in Controller.I.CurrentTests)
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }

            Console.ReadKey();
        }
    }
}