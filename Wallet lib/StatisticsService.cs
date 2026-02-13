using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_lib
{
    public class StatisticsService
    {
        /// <summary>
        ///  Calculates the coefficients of the line that best fits the given data points
        /// </summary>
        /// <param name="monthsBalance">A list of monthly balances.</param>
        /// <returns>A tuple containing the slope (a) and intercept (b) of the fitted line.</returns>
        /// <exception cref="Exception">Thrown when the input list is empty.</exception>
        public static (double, double) Line_Fit(List<decimal> monthsBalance)
        {
            int n = monthsBalance.Count();
            if (n <2)
            {
                throw new Exception();
            }

            List<int> xValues = new();
            for (int i = 0; i < n; i++)
            {
                xValues.Add(i);
            }
            double EX = (double)xValues.Sum() / n;
            double EY = (double)monthsBalance.Sum() / n;
            double EXY = 0;
            for (int i = 0; i < n; i++)
            {
                EXY += (double)monthsBalance[i] * xValues[i];
            }
            EXY = EXY / n;
            double var_x = 0;
            foreach (var x in xValues)
            {
                var_x += (x - EX) * (x - EX);
            }
            var_x = var_x / n;
            //coefficients of the fitted line
            double a = (EXY - EX * EY) / var_x;
            double b = EY - a * EX;
            return (a, b);
        }
    }
}
