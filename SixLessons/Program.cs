using static System.Console;
public class Account
{
    private decimal _balance;
    private readonly object _lock = new object(); // неизменяемая ссылка на объект

    public decimal Balance
    {
        get
        {
            lock (_lock)
            { 
                return _balance;
            }
        }
    }

    public Account(decimal iBalance)
    {
        _balance = iBalance;
    }

    public void Credit(decimal amount)
    {
        lock (_lock)
        {
            _balance += amount;
            WriteLine($"Пополнили: + {amount}, Баланс: {_balance}");
        }
    }

    public bool Debit(decimal amount)
    {
        lock (_lock)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                WriteLine($"Списали: -{amount}, Баланс: {_balance}");
                return true;
            }
            else
            {
                WriteLine($"ERROR: Нельзя снять {amount}, есть только {_balance}");
                return false;
            }
        }
    }
}
class Program
{
    static void Main()
    {
        var account = new Account(1000);
        account.Credit(1000);
        
        for (int i = 0; i < 4; i++)
        {
            int threadId = i;
            Thread thread = new Thread(() =>
            {
                WriteLine($"Поток {threadId}");


                if (account.Debit(1000))
                {
                    WriteLine($"Поток {threadId} успешно снял деньги");
                }
                else
                {
                    WriteLine($"Поток {threadId} не смог снять деньги");
                }

            });
            thread.Start();
        }

        Thread.Sleep(100);
        WriteLine($"\nИтоговый баланс: {account.Balance}");
    }
}