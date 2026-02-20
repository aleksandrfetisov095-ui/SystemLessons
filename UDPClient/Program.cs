using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;

using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);// тип адресации,сокета,протокол


EndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555); // адресная точка, место, куда отправляют данные


Console.WriteLine("Клиент запущен");


double[] xValues = { 1.0, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0, 5.5, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0 };
double[] yValues = { 1.8, 2.0, 1.0, 2.5, 3.0, 3.0, 1.0, 3.5, 3.0, 4.0, 1.0, 4.5, 3.0, 5.0, 1.0, 5.5, 3.0, 6.0, 1.0, 6.5, 3.0, 7.0, 1.0, 7.5 };

// отправляет все точки на сервер по очереди
for (int i = 0; i < xValues.Length; i++)
{
    string message = $"{xValues[i]} {yValues[i]}";
    byte[] data = Encoding.UTF8.GetBytes(message); // преобразуем в байты
    int bytes = await udpSocket.SendToAsync(data, serverPoint); // ассинхронно отправка данных через UDP - сокет
    Console.WriteLine($"Отправлено: {message} ({bytes} байт)");
    await Task.Delay(1000);
}

while (true)
{
    Console.Write("> ");
    string? text = Console.ReadLine();

    if (text?.ToLower() == "exit")
        break;

    if (!string.IsNullOrEmpty(text))
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        int bytes = await udpSocket.SendToAsync(data, serverPoint);
        Console.WriteLine($"Отправлено {bytes} байт");
    }
}

Console.WriteLine("Клиент завершил работу");