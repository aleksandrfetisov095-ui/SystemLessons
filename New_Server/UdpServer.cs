using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace New_Server
{
    public class UdpServer
    {
        private readonly int _port;
        private readonly CommandHandler _commandHandler;
        private Socket _udpSocket;
        private bool _isRunning;

        public UdpServer(int port, CommandHandler commandHandler)
        {
            _port = port;
            _commandHandler = commandHandler;
        }

        public async Task StartAsync()
        {
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);
            _udpSocket.Bind(localIP);
            _isRunning = true;

            Console.WriteLine($"\n UDP Сервер запущен на 127.0.0.1:{_port}");
            Console.WriteLine($" Ожидание данных...\n");

            while (_isRunning)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    // Получаем данные
                    var result = _udpSocket.ReceiveFrom(buffer, ref clientEndPoint);
                    string message = Encoding.UTF8.GetString(buffer, 0, result);

                    // Обрабатываем команду
                    string response = await _commandHandler.ProcessMessageAsync(message, clientEndPoint.ToString());

                    // Отправляем ответ
                    if (!string.IsNullOrEmpty(response))
                    {
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        _udpSocket.SendTo(responseData, clientEndPoint);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Ошибка: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _udpSocket?.Close();
        }
    }
}
