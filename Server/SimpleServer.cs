using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server;

public class SimpleServer : IDisposable
{
    private readonly IPEndPoint _ipEndPoint;
    private readonly int _backlog;

    private const int defaultSizeBuffer = 1024;
    private readonly int _sizeBuffer;

    private readonly Socket _serverSocket;
    private readonly ApplicationContext applicationContext = new();

    public SimpleServer(IPEndPoint ipEndPoint, int backlog)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _backlog = backlog;

        _sizeBuffer = defaultSizeBuffer;

        _serverSocket = Configuration();
    }

    public SimpleServer(IPEndPoint ipEndPoint, int backlog, int sizeBuffer)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _backlog = backlog;

        _sizeBuffer = sizeBuffer;

        _serverSocket = Configuration();
    }

    public async Task RunAsync()
    {
        Socket? clientSocket = null;

        try
        {
            while (true)
            {
                clientSocket = await _serverSocket.AcceptAsync();
                var request = await HandlerReceiveAsync(clientSocket);

                if (request.ToLower() == "get data")
                {
                    var products = await applicationContext.Books.ToListAsync();
                    var serializeProducts = JsonSerializer.Serialize(products);
                    await HandlerSendAsync(clientSocket, serializeProducts);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            clientSocket?.Dispose();
        }
    }

    private async Task<string> HandlerReceiveAsync(Socket clientSocket)
    {
        int readBytes;
        var buffer = new byte[_sizeBuffer];
        var requestBuilder = new StringBuilder();
        do
        {
            readBytes = await clientSocket.ReceiveAsync(buffer);
            var request = Encoding.UTF8.GetString(buffer, 0, readBytes);
            requestBuilder.Append(request);
        }
        while (readBytes > 0);
        clientSocket.Shutdown(SocketShutdown.Receive);

        return requestBuilder.ToString();
    }

    private static async Task HandlerSendAsync(Socket clientSocket, string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        await clientSocket.SendAsync(messageBytes);
        clientSocket.Shutdown(SocketShutdown.Send);
    }

    private Socket Configuration()
    {
        var serverSoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSoket.Bind(_ipEndPoint);
        serverSoket.Listen(_backlog);

        return serverSoket;
    }

    public void Dispose()
    {
        _serverSocket.Dispose();
    }
}
