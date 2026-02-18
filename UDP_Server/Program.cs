using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Data.SqlClient;

using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
udpSocket.Bind(localIP);
Console.WriteLine("Сервер запущен. Ожидание данных...");

string connectionString = "Server=DESKTOP-Q5BCN1Q;Database=P_320_CompanyDB_Fetisov;Trusted_Connection=True;";


using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    // Создаем таблицу (если её нет)
    var createTable = connection.CreateCommand();
    createTable.CommandText = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='messages' AND xtype='U')
        CREATE TABLE messages (
            id INT IDENTITY(1,1) PRIMARY KEY,
            text NVARCHAR(MAX) NOT NULL,
            time DATETIME NOT NULL
        )";
    createTable.ExecuteNonQuery();

    Console.WriteLine("Подключение к SQL Server установлено");
}

while (true)
{
    try
    {
        // Буфер для получения данных
        byte[] buffer = new byte[1024];
        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // Получаем данные
        var result = await udpSocket.ReceiveFromAsync(buffer, clientEndPoint);
        string message = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);

        Console.WriteLine($"Получено: {message} от {result.RemoteEndPoint}");

        // Сохраняем в базу данных SQL Server
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO messages (text, time) VALUES (@text, @time)";
            insert.Parameters.AddWithValue("@text", message);
            insert.Parameters.AddWithValue("@time", DateTime.Now);
            insert.ExecuteNonQuery();
        }

        Console.WriteLine("Данные сохранены в SQL Server");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}