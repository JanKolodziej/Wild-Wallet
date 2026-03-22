namespace Wallet_lib
{
    public class StatisticsService
    {
        /// <summary>
        ///  Calculates the coefficients of the line that best fits the given data points.
        ///  Uses x=0,1,2,... as x values
        /// </summary>
        /// <param name="monthsBalance">A list of monthly balances.</param>
        /// <returns>A tuple containing the slope (a) and intercept (b) of the fitted line.</returns>
        /// <exception cref="Exception">Thrown when the input list is empty.</exception>
        public static (double, double) Line_Fit(List<double> monthsBalance)
        {
            int n = monthsBalance.Count;
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
        /// <summary>
        /// Calculates the Uncertainty of the fit
        /// </summary>
        /// <param name="actualValues"></param>
        /// <param name="predictedValues"></param>
        /// <returns></returns>
        public static double Uncertainty(List<double> actualValues, List<double> predictedValues)
        {
            int n = actualValues.Count();
            if (n <= 2)
            {
                return 0;
            }
            double S = 0;
            double u_a = 0;
            double u_b = 0;
            double sum_x2 = 0;
            double sum_x = 0;

            for (int i = 0; i < n; i++)
            {
                S += Math.Pow(((double)actualValues[i] - predictedValues[i]), 2);
                sum_x2 += i * i;
                sum_x += i;
            }
            S = Math.Sqrt(S / (n - 2));
            u_a = S * Math.Sqrt(n / ((n * sum_x2) - sum_x * sum_x));
            u_b = S * Math.Sqrt(sum_x2 / ((n * sum_x2) - sum_x * sum_x));
            return Math.Sqrt(u_a * u_a + u_b * u_b);
        }



        //TO DO 
        // Napisać funckje która przewiduje jaki powinien być budżet 
        // Zrobić regresje liniową przez wszystkie tranzakcje z danej kategorii w miesiącach
        // i na podstawie tego przewiduje wartość w tym miesiącu
        // Można dodać jakiś współczynnik zmniejszający wynik tak aby użytkownik dążył do niżeszej kwoty np 0.95


    }
}



