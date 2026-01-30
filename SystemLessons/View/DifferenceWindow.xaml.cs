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
using System.Windows.Shapes;
using SystemLessons.ViewModel;
namespace SystemLessons.View
{
    /// <summary>
    /// Логика взаимодействия для DifferenceWindowVm.xaml
    /// </summary>
    public partial class DifferenceWindow : Window 
    {
        public DifferenceWindow()
        {
            InitializeComponent();
            DataContext = new DifferenceWindowVm(ShowMessage);
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Разность", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
