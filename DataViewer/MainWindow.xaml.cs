using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Microsoft.Data.SqlClient;

namespace DataViewer
{
    public partial class MainWindow : Window
    {
        
        private string connectionString = "Server=DESKTOP-Q5BCN1Q;Database=P_320_CompanyDB_Fetisov;Trusted_Connection=True;";
        private string serverIp = "127.0.0.1";
        private int serverPort = 5555;

        // для хранения сообщений
        private List<MessageItem> messages = new List<MessageItem>();

        public MainWindow()
        {
            InitializeComponent();
            LoadMessagesAsync();
        }

        
        // работа с бд
        private async void LoadFromDatabase_Click(object sender, RoutedEventArgs e) // загрузить из бд
        {
            await LoadMessagesAsync();
        }

        private async Task LoadMessagesAsync() // получение данных из бд и отображение
        {
            try
            {
                StatusText.Text = "Загрузка данных из БД...";
                LoadingProgressBar.Visibility = Visibility.Visible;

                messages.Clear();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand(
                        "SELECT id, text, time FROM messages ORDER BY time DESC",
                        connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            messages.Add(new MessageItem
                            {
                                Id = reader.GetInt32(0),
                                Content = reader.GetString(1),  
                                ReceivedTime = reader.GetDateTime(2) 
                                
                            });
                        }
                    }
                }

                // Обновление таблицы
                MessagesDataGrid.ItemsSource = null;
                MessagesDataGrid.ItemsSource = messages;

                // Обновление счетчика
                CounterText.Text = $"Всего записей: {messages.Count}";
                StatusText.Text = $"Загружено {messages.Count} записей";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки из БД: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка загрузки";
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        
        //работа с файлами
        private void ExportToCsv_Click(object sender, RoutedEventArgs e) // экспорт в эксель
        {
            if (messages.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Сохранить как CSV",
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"messages_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    StatusText.Text = "Экспорт данных...";
                    LoadingProgressBar.Visibility = Visibility.Visible;

                    var sb = new StringBuilder();

                    sb.AppendLine("ID;Сообщение;Время получения");

                    // Данные
                    foreach (var msg in messages)
                    {
                        string content = msg.Content.Replace("\"", "\"\"");
                        if (content.Contains(";") || content.Contains("\""))
                        {
                            content = $"\"{content}\"";
                        }

                        sb.AppendLine($"{msg.Id};{content};{msg.ReceivedTime:yyyy-MM-dd HH:mm:ss}");
                    }

                    // Сохраняем в файл
                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);

                    StatusText.Text = $"Экспорт завершен: {System.IO.Path.GetFileName(dialog.FileName)}";

                    MessageBox.Show($"Данные экспортированы в файл:\n{dialog.FileName}",
                        "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        // работа с сетью

        private async void SendTestData_Click(object sender, RoutedEventArgs e) // отправка данных
        {
            try
            {
                StatusText.Text = "Отправка тестовых данных...";
                LoadingProgressBar.Visibility = Visibility.Visible;

                using (var udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    var serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

                    string[] testMessages = new[]
                    {
                        "Привет от WPF клиента!",
                        $"Тестовое сообщение {DateTime.Now:HH:mm:ss}",
                        "UDP работает!",
                        "Курсовой проект",
                        $"Случайное число: {new Random().Next(1, 100)}"
                    };

                    foreach (var msg in testMessages)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(msg);
                        udpClient.SendTo(data, serverEndPoint); // Синхронная отправка

                        StatusText.Text = $"Отправлено: {msg}";
                        await Task.Delay(500);
                    }
                }

                StatusText.Text = "Тестовые данные отправлены";
                await Task.Delay(1000);
                await LoadMessagesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки: {ex.Message}\n\nУбедитесь, что сервер запущен!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка отправки";
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        // очистка бд

        private async void ClearDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите очистить все сообщения из базы данных?",
                "Подтверждение очистки",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Очистка базы данных...";
                    LoadingProgressBar.Visibility = Visibility.Visible;

                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        var command = new SqlCommand("DELETE FROM messages", connection);
                        int deletedCount = await command.ExecuteNonQueryAsync();

                        StatusText.Text = $"Удалено записей: {deletedCount}";
                    }

                    await LoadMessagesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка очистки БД: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void MessagesDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }

    public class MessageItem
    {
        public int Id { get; set; }
        public string Content { get; set; } 
        public DateTime ReceivedTime { get; set; }
       
    }
}