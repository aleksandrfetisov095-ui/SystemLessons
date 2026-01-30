using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLessons.Model
{
    public static class Calculator
    {
        //сумма с task
        public static async Task<double> GetSumAsync(double number, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                double sum = 0;

                for (int i = 1; i <= number; i++)
                {
                    
                    if (cancellationToken.IsCancellationRequested)
                        return 0; 

                    sum += i;

                    
                    Thread.Sleep(1);
                }

                return sum; 
            }, cancellationToken);
        }

        //разность с task
        public static async Task<double> GetDifferenceAsync(double number, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                double difference = 0;

                for (int i = 1; i <= number; i++)
                {
                    
                    if (cancellationToken.IsCancellationRequested)
                        return 0;

                    difference -= i;

                    
                    Thread.Sleep(1);

                }

                return Math.Abs(difference);
            }, cancellationToken);
        }

    }
}
