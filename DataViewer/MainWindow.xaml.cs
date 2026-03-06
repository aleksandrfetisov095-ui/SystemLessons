using DataViewer.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DataViewer
{
    public partial class MainWindow : Window
    {
        private List<MessageDisplay> messages = new List<MessageDisplay>();
        private readonly UdpClientService _udpService;

        public MainWindow()
        {
            InitializeComponent();
            string serverIp = ConfigurationManager.AppSettings["ServerIp"] ?? "127.0.0.1";
            int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"] ?? "5555");
            _udpService = new UdpClientService(serverIp, serverPort);

            _ = LoadMessagesFromServerAsync();
        }

       
        private async Task LoadMessagesFromServerAsync()
        {
            try
            {
                StatusText.Text = "Запрос данных с сервера...";
                LoadingProgressBar.Visibility = Visibility.Visible;

                messages = await _udpService.GetAllMessagesAsync();

                // Обновляем таблицу
                MessagesDataGrid.ItemsSource = null;
                MessagesDataGrid.ItemsSource = messages;

                // Обновляем счетчик
                CounterText.Text = $"Всего записей: {messages.Count}";
                StatusText.Text = $"Загружено {messages.Count} записей";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка загрузки";
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        
        private async void LoadFromServer_Click(object sender, RoutedEventArgs e)
        {
            await LoadMessagesFromServerAsync();
        }

        
        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
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

                    foreach (var msg in messages)
                    {
                        string content = msg.Content.Replace("\"", "\"\"");
                        if (content.Contains(";") || content.Contains("\""))
                        {
                            content = $"\"{content}\"";
                        }

                        sb.AppendLine($"{msg.Id};{content};{msg.ReceivedTime:yyyy-MM-dd HH:mm:ss}");
                    }

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

      
        private async void SendTestData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Отправка тестовых данных...";
                LoadingProgressBar.Visibility = Visibility.Visible;

                var testMessages = _udpService.GetTestMessages();

                foreach (var msg in testMessages)
                {
                    _udpService.SendMessage(msg);
                    StatusText.Text = $"Отправлено: {msg}";
                    await Task.Delay(500);
                }

                StatusText.Text = "Тестовые данные отправлены";
                await Task.Delay(1000);
                await LoadMessagesFromServerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки: {ex.Message}\n\nУбедитесь, что сервер NEW_SERVER запущен!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка отправки";
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

       
        private async void ClearServerData_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите очистить все сообщения на сервере?",
                "Подтверждение очистки",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Очистка данных на сервере...";
                    LoadingProgressBar.Visibility = Visibility.Visible;

                    int deletedCount = await _udpService.ClearAllMessagesAsync();

                    StatusText.Text = $"Удалено записей: {deletedCount}";
                    await LoadMessagesFromServerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка очистки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}