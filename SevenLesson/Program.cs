using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
namespace SevenLesson
{
    public class Program
    {
        static void Factorial(int n)
        {
            int factor = 1;
            for (int i = 2; i < n; i++)
            {
                factor *= i;
            }
            WriteLine($"Факториал {n} = {factor}");

        }
        static void Fibonacci(int n)
        {
            long result = Calc(n);
            WriteLine($"(поток: {Task.CurrentId}) , F({n}) = {result}");
            Thread.Sleep(10); 
        }

        static long Calc(int n)
        {
            if (n <= 1) 
            {
                return n; 
            }
            long a = 0, b = 1, result = 0;
            for (int i = 2; i <= n; i++)
            {
                result = a + b;
                a = b;
                b = result;
            }
            return result;
        }

        static void Main(string[] args)
        {
            // 1.1 задача факториалы -----------------------------------
            //ParallelLoopResult res = Parallel.For(1, 10, Factorial);
            //if (res.IsCompleted)
            //{
            //    WriteLine(" Успешно!");
            //}
            //else
            //{
            //    WriteLine($" Прерван на итерации {res.LowestBreakIteration}");
            //}
            // 1.2 задача фибоначи ---------------------------------------
            //var numbers = new List<int> { 2, 3, 4, 5 };


            //Parallel.ForEach(numbers, Fibonacci);

            //WriteLine("\n Все вычисления завершены!");
            // Задача 2.1 четные числа -------------------------------------
            //var data = Enumerable.Range(0, 100000);
            //var result = data.AsParallel()
            //                .Where(x => x % 2 == 0)
            //                .Count();
            //WriteLine($"Четных чисел: {result}");
            // Задача 2.2 Простые числа ---------------------------------------------------
            //var result = Enumerable.Range(2, 1_000_000)
            //                  .AsParallel()
            //                  .Where(x =>
            //                  {
            //                      for (int i = 2; i * i <= x; i++)
            //                          if (x % i == 0) return false;
            //                      return true;
            //                  })
            //                  .ToList();

            //WriteLine($"Простых чисел: {result.Count}");
            //Задача 2.3 сумма квадратов
            var data = Enumerable.Range(0, 1000);

            var res = data.AsParallel()
                            .Select(x => x * 2) 
                            .Sum();

            WriteLine($"Сумма: {res}");
        }
        
    }
}
