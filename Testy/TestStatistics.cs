using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;
namespace Testy
{
    public class TestStatistics
    {
        [Fact]
        public void CalculateLinearRegression_PerfectRisingLine_ReturnsCorrectCoefficients()
        {
            // ARRANGE (Przygotowanie)
            // Prosta y = 1x + 0 (czyli y = x)
            double[] x = { 1, 2, 3, 4, 5 };
            List<decimal> y = new() {0, 1, 2, 3, 4, 5 };
            double a, b;
            // ACT (Działanie)
             (a,b) = StatisticsService.Line_Fit(y);

            // ASSERT (Sprawdzenie)
            // Oczekujemy a = 1, b = 0
            Assert.Equal(1.0, a, precision: 5); // precision to margines błędu dla double
            Assert.Equal(0.0, b, precision: 5);
        }

        [Fact]
        public void CalculateLinearRegression_DecreasingLine_ReturnsNegativeSlope()
        {
            //  y = -2x + 10
            List<decimal> y = new() {10, 8, 6, 4 };

            // ACT
            double a, b;

            (a, b) = StatisticsService.Line_Fit(y);

            // ASSERT
            // Oczekujemy a = -2, b = 10
            Assert.Equal(-2.0, a, precision: 5);
            Assert.Equal(10.0, b, precision: 5);
        }
    }
}
