using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Data.SqlClient;


string connectionString = "Server=DESKTOP-Q5BCN1Q;Database=P_320_CompanyDB_Fetisov;Trusted_Connection=True;";
int port = 5555;

try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Подключение к БД успешно");

        var checkTable = connection.CreateCommand();
        checkTable.CommandText = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='messages' AND xtype='U')
            CREATE TABLE messages (
                id INT IDENTITY(1,1) PRIMARY KEY,
                text NVARCHAR(MAX) NOT NULL,
                time DATETIME NOT NULL
            )";
        checkTable.ExecuteNonQuery();
        Console.WriteLine("Таблица messages проверена/создана");
    }

    using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
    udpSocket.Bind(localIP);

    Console.WriteLine($"UDP Сервер запущен на 127.0.0.1:{port}");
    Console.WriteLine($"Ожидание данных...");
    

    //прием данных
    while (true)
    {
        try
        {
            // Буфер для получения данных
            byte[] buffer = new byte[1024];
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Получаем данные
            var result = udpSocket.ReceiveFrom(buffer, ref clientEndPoint);
            string message = Encoding.UTF8.GetString(buffer, 0, result);

            string clientInfo = clientEndPoint.ToString(); // ip_adress, порт
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Получено: '{message}' от {clientInfo}");

            //сохранение в бд
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var insert = connection.CreateCommand();
                insert.CommandText = "INSERT INTO messages (text, time) VALUES (@text, @time)";
                insert.Parameters.AddWithValue("@text", message);
                insert.Parameters.AddWithValue("@time", DateTime.Now);

                insert.ExecuteNonQuery();
                Console.WriteLine($"  -> Сохранено в БД");
            }

            //сохранение в файл
            string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:MM:ss}] {message} от {clientInfo}{Environment.NewLine}";
            File.AppendAllText("received_messages.log", logLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке: {ex.Message}");
            File.AppendAllText("errors.log", $"[{DateTime.Now}] Ошибка: {ex.Message}{Environment.NewLine}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Критическая ошибка: {ex.Message}");
    Console.WriteLine("Нажмите любую клавишу для выхода...");
    Console.ReadKey();
}