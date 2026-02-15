using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class Program
{
    private const int port = 8888;
    private const int backlog = 10;

    public async static Task Main()
    {
        Console.WriteLine("Сервер запущен");
        var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
        var simpleServer = new SimpleServer(ipEndPoint, backlog);

        try
        {
            await simpleServer.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            simpleServer.Dispose();
        }
    }
}