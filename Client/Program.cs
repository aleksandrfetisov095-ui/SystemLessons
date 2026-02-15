using Core;
using System.Net;
namespace Client;

public class Program
{
    private const int port = 8888;
    private readonly static byte[] arrayIpAddress = [127, 0, 0, 1];

    public static async Task Main()
    {
        Console.WriteLine("Клиент запущен");

        //Сериалоизация и десериализация

        var ipAddress = new IPAddress(arrayIpAddress);
        var ipEndPoint = new IPEndPoint(ipAddress, port);
        var simpleClient = new SimpleClient(ipEndPoint);

        while (true)
        {
            var message = Console.ReadLine();

            if (string.IsNullOrEmpty(message))
            {
                continue;
            }

            var products = await simpleClient.SendAsync<List<bookShop>>(message);

            foreach (var product in products)
            {
                Console.WriteLine(product);
            }
        }
    }
}