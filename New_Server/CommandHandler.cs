using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Server
{
    public class CommandHandler
    {
        private readonly DatabaseService _dbService;

        public CommandHandler(DatabaseService dbService)
        {
            _dbService = dbService;
        }
        public async Task<string> ProcessMessageAsync(string message, string clientAddress)
        {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] Получено: '{message}' от {clientAddress}");
            if (message.StartsWith("CMD:"))
            {
                return await ProcessCommandAsync(message);
            }
            else
            {
                return await ProcessDataAsync(message);
            }
        }
        private async Task<string> ProcessCommandAsync(string message)
        {
            string command = message.Substring(4).ToUpper();

            switch (command)
            {
                case "GET_ALL":
                    var allMessages = await _dbService.GetAllMessagesAsync();
                    return EncodeMessagesResponse(allMessages);

                case "GET_LAST":
                    var lastMessages = await _dbService.GetLastMessagesAsync(10);
                    return EncodeMessagesResponse(lastMessages);

                case "GET_COUNT":
                    int count = await _dbService.GetMessagesCountAsync();
                    return $"RESPONSE:COUNT={count}";

                case "CLEAR":
                    int deleted = await _dbService.ClearAllMessagesAsync();
                    Console.WriteLine($" Очищено {deleted} записей");
                    return $"RESPONSE:CLEARED={deleted}";

                default:
                    return "RESPONSE:ERROR:UNKNOWN_COMMAND";
            }
        }
        private async Task<string> ProcessDataAsync(string message)
        {
            int id = await _dbService.SaveMessageAsync(message);

            string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            System.IO.File.AppendAllText("received_messages.log", logLine);

            Console.WriteLine($" Сохранено в БД (ID: {id})");

            return $"RESPONSE:SAVED:ID={id}";
        }
        private string EncodeMessagesResponse(System.Collections.Generic.List<MessageData> messages)
        {
            var sb = new StringBuilder("RESPONSE:MESSAGES:");
            foreach (var msg in messages)
            {
                sb.Append($"{msg.Id}|{EscapeText(msg.Text)}|{msg.Time:yyyy-MM-dd HH:mm:ss};");
            }
            return sb.ToString();
        }
        private string EscapeText(string text)
        {
            return text.Replace("|", "[slash]").Replace(";", "[dot]");
        }
    }
}
