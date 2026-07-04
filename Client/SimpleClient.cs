using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client;

public class SimpleClient
{
    private readonly IPEndPoint _ipEndPoint;
    private const int defaultSizeBuffer = 1024;
    private int _sizeBuffer;
    public SimpleClient(IPEndPoint ipEndPoint)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");
        _ipEndPoint = ipEndPoint;
        _sizeBuffer = defaultSizeBuffer;
    }

    public SimpleClient(IPEndPoint ipEndPoint, int sizeBuffer)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _sizeBuffer = sizeBuffer;
    }

    public async Task<T?> SendAsync<T>(string message)
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);
            await HandlerSendAsync(clientSocket, message);
            return await HandlerReceiveAsync<T>(clientSocket);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    private static async Task HandlerSendAsync(Socket clientSocket, string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        await clientSocket.SendAsync(messageBytes);
        clientSocket.Shutdown(SocketShutdown.Send);
    }

    private async Task<T?> HandlerReceiveAsync<T>(Socket clientSocket)
    {
        int readBytes;
        var buffer = new byte[_sizeBuffer];
        var responceBuilder = new StringBuilder();
        do
        {
            readBytes = await clientSocket.ReceiveAsync(buffer);
            var responce = Encoding.UTF8.GetString(buffer, 0, readBytes);
            responceBuilder.Append(responce);
        }
        while (readBytes > 0);

        clientSocket.Shutdown(SocketShutdown.Receive);

        return JsonSerializer.Deserialize<T>(responceBuilder.ToString());
    }
}
