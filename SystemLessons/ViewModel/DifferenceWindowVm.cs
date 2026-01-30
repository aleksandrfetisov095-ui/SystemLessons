using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemLessons.Model;

namespace SystemLessons.ViewModel
{
    public class DifferenceWindowVm : BaseVm
    {

        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();

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

        public DifferenceWindowVm(Action<string> showMessage)
        {
            _showMessage = showMessage;

            CalculateCommand = new RelayCommand(CalculateAsync, _ => !IsCalculating);
            CancelCommand = new RelayCommand(Cancel, _ => IsCalculating);
        }

        private async void CalculateAsync(object parameter)
        {
            if (!ValidateInput())
                return;

            IsCalculating = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Используем Task для вычисления разности
                double result = await Calculator.GetDifferenceAsync(
                    InputNumber,
                    _cancellationTokenSource.Token
                );

                Result = result;
                _showMessage?.Invoke($"Разность вычислена: {result}");
            }
            catch (OperationCanceledException)
            {
                Result = 0;
                _showMessage?.Invoke("Вычисление разности отменено!");
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

            InputNumber = number;
            return true;
        }
        private void Cancel(object parameter)
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
