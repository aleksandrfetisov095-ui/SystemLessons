using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemLessons.Model;

namespace SystemLessons.ViewModel
{
    public class SumWindowVm : BaseVm
    {
        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();

                // Автоматическое преобразование
                if (double.TryParse(value, out double number))
                {
                    InputNumber = number;
                }
            }
        }

        private double _inputNumber;
        public double InputNumber
        {
            get => _inputNumber;
            set
            {
                _inputNumber = value;
                OnPropertyChanged();
            }
        }

        private double _result;
        public double Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        private bool _isCalculating;
        public bool IsCalculating
        {
            get => _isCalculating;
            set
            {
                _isCalculating = value;
                OnPropertyChanged();
                CalculateCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand CalculateCommand { get; }
        public RelayCommand CancelCommand { get; }

        private readonly Action<string> _showMessage;
        private CancellationTokenSource _cancellationTokenSource;

        public SumWindowVm(Action<string> showMessage)
        {
            _showMessage = showMessage;

            CalculateCommand = new RelayCommand(CalculateAsync, _ => !IsCalculating);
            CancelCommand = new RelayCommand(Cancel, _ => IsCalculating);
        }

        private async void CalculateAsync(object parameter)
        {
            // Проверка ввода
            if (!ValidateInput())
                return;

            IsCalculating = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Используем async/await с Task
                double result = await Calculator.GetSumAsync(
                    InputNumber,
                    _cancellationTokenSource.Token
                );

                Result = result;
                _showMessage?.Invoke($"Сумма вычислена: {result}");
            }
            catch (OperationCanceledException)
            {
                Result = 0;
                _showMessage?.Invoke("Вычисление суммы отменено!");
            }
            catch (Exception ex)
            {
                _showMessage?.Invoke($"Ошибка: {ex.Message}");
            }
            finally
            {
                IsCalculating = false;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                _showMessage?.Invoke("Введите число!");
                return false;
            }

            if (!double.TryParse(InputText, out double number))
            {
                _showMessage?.Invoke($"'{InputText}' - это не число!");
                return false;
            }

            if (number <= 0)
            {
                _showMessage?.Invoke("Введите положительное число!");
                return false;
            }

            InputNumber = number;
            return true;
        }

        private void Cancel(object parameter)
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
