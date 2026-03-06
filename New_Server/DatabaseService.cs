using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Server
{

    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InitializeDatabaseAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine(" Подключение к БД успешно");

                var createTable = connection.CreateCommand();
                createTable.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='messages' AND xtype='U')
                    CREATE TABLE messages (
                        id INT IDENTITY(1,1) PRIMARY KEY,
                        text NVARCHAR(MAX) NOT NULL,
                        time DATETIME NOT NULL
                    )";
                await createTable.ExecuteNonQueryAsync();

                Console.WriteLine(" Таблица messages проверена/создана");
            }
        }

      
        public async Task<int> SaveMessageAsync(string text)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var insert = connection.CreateCommand();
                insert.CommandText = @"INSERT INTO messages (text, time) 
                                      OUTPUT INSERTED.id 
                                      VALUES (@text, @time)";
                insert.Parameters.AddWithValue("@text", text);
                insert.Parameters.AddWithValue("@time", DateTime.Now);

                return (int)await insert.ExecuteScalarAsync();
            }
        }


        public async Task<List<MessageData>> GetAllMessagesAsync()
        {
            var messages = new List<MessageData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(
                    "SELECT id, text, time FROM messages ORDER BY time DESC",
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        messages.Add(new MessageData
                        {
                            Id = reader.GetInt32(0),
                            Text = reader.GetString(1),
                            Time = reader.GetDateTime(2)
                        });
                    }
                }
            }

            return messages;
        }//
        public async Task<List<MessageData>> GetLastMessagesAsync(int count)
        {
            var allMessages = await GetAllMessagesAsync();
            return allMessages.GetRange(0, Math.Min(count, allMessages.Count));
        }

        
        public async Task<int> ClearAllMessagesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("DELETE FROM messages", connection);
                return await command.ExecuteNonQueryAsync();
            }
        }
        //
        public async Task<int> GetMessagesCountAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT COUNT(*) FROM messages", connection);
                return (int)await command.ExecuteScalarAsync();
            }
        }
    }
    public class MessageData
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}
