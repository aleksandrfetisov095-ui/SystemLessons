using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EightLesson
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;

            // Проверка
            bool isValid = EmailValidator.IsValid(email);

            if (isValid)
            {
                ResultText.Text = " Email правильный!";
                ResultText.Foreground = Brushes.Green; // цвет
                ResultText.FontWeight = FontWeights.Bold; // жирность
            }
            else
            {
                // Проверка с ошибками
                string errorMessage = EmailValidator.CheckEmail(email);
                ResultText.Text = errorMessage;
                ResultText.Foreground = Brushes.Red;
            }
        }

        // Нажали на пример
        private void Example_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button; // преобразование кнопки
            if (button != null)
            {
                EmailBox.Text = button.Content.ToString();
                CheckButton_Click(null, null); // автоматически проверяем
            }
        }
    }
}
