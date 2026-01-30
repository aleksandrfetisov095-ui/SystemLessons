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
using SystemLessons.View;

namespace SystemLessons
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

        private void OpenSumWindow_Click(object sender, RoutedEventArgs e)
        {
            var sumWindow = new SumWindow(); 
            sumWindow.Show();
        }

        private void OpenDifferenceWindow_Click(object sender, RoutedEventArgs e)
        {
            var diffWindow = new DifferenceWindow();
            diffWindow.Show();
        }

        private void OpenBothWindows_Click(object sender, RoutedEventArgs e)
        {
            OpenSumWindow_Click(sender, e);
            OpenDifferenceWindow_Click(sender, e);
        }
    }
}
