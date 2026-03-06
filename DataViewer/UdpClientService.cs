using DataViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DataViewer
{
    public class UdpClientService
    {
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly IPEndPoint _serverEndPoint;

        public UdpClientService(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        }
        public async Task<string> SendAndReceiveAsync(string message)
        {
            using (var udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                byte[] sendData = Encoding.UTF8.GetBytes(message);
                await Task.Run(() => udpClient.SendTo(sendData, _serverEndPoint));

                
                byte[] receiveBuffer = new byte[8192];
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                var receiveTask = Task.Run(() => udpClient.ReceiveFrom(receiveBuffer, ref remoteEndPoint));
                if (await Task.WhenAny(receiveTask, Task.Delay(3000)) == receiveTask)
                {
                    int received = receiveTask.Result;
                    return Encoding.UTF8.GetString(receiveBuffer, 0, received);
                }

                return null;
            }
        }

        // Отправка без ожидания ответа
        public void SendMessage(string message)
        {
            using (var udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                udpClient.SendTo(data, _serverEndPoint);
            }
        }

        // Получение всех сообщений с сервера
        public async Task<List<MessageDisplay>> GetAllMessagesAsync()
        {
            var messages = new List<MessageDisplay>();

            string response = await SendAndReceiveAsync("CMD:GET_ALL");

            if (response != null && response.StartsWith("RESPONSE:MESSAGES:"))
            {
                string data = response.Replace("RESPONSE:MESSAGES:", "");
                string[] items = data.Split(';', (char)StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in items)
                {
                    string[] parts = item.Split('|');
                    if (parts.Length == 3)
                    {
                        messages.Add(new MessageDisplay
                        {
                            Id = int.Parse(parts[0]),
                            Content = UnescapeText(parts[1]),
                            ReceivedTime = DateTime.Parse(parts[2])
                        });
                    }
                }
            }

            return messages;
        }

        public async Task<int> GetMessagesCountAsync()
        {
            string response = await SendAndReceiveAsync("CMD:GET_COUNT");

            if (response != null && response.StartsWith("RESPONSE:COUNT="))
            {
                return int.Parse(response.Replace("RESPONSE:COUNT=", ""));
            }

            return 0;
        }

        public async Task<int> ClearAllMessagesAsync()
        {
            string response = await SendAndReceiveAsync("CMD:CLEAR");

            if (response != null && response.StartsWith("RESPONSE:CLEARED="))
            {
                return int.Parse(response.Replace("RESPONSE:CLEARED=", "")); // оставляем только число
            }
            return 0;
        }
        private string UnescapeText(string text)
        {
            return text.Replace("[slash]", "|").Replace("[dot]", ";");
        }

        // Тестовые сообщения
        public string[] GetTestMessages()
        {
            return new[]
            {
                "Привет!!!",
                $"Тестовое сообщение {DateTime.Now:HH:mm:ss}",
                "UDP работает!",
                "Курсовой проект",
                $"Случайное число: {new Random().Next(1, 100)}"
            };
        }
    }
}
