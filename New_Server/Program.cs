using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine(" UDP СЕРВЕР С БАЗОЙ ДАННЫХ");
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                int port = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);

                // Создаем сервисы
                var dbService = new DatabaseService(connectionString);
                await dbService.InitializeDatabaseAsync();

                var commandHandler = new CommandHandler(dbService);
                var udpServer = new UdpServer(port, commandHandler);

                // Запуск
                await udpServer.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine("Нажмите любую кнопку для выхода...");
                Console.ReadKey();
            }
        }
    }
}
