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
            List<double> y = new() {0, 1, 2, 3, 4, 5 };
            double a, b;
             (a,b) = StatisticsService.Line_Fit(y);


            Assert.Equal(1.0, a, 5) ; 
            Assert.Equal(0.0, b, 5);
        }

        [Fact]
        public void CalculateLinearRegression_DecreasingLine_ReturnsNegativeSlope()
        {
            List<double> y = new() {10, 8, 6, 4 };

            double a, b;

            (a, b) = StatisticsService.Line_Fit(y);

            Assert.Equal(-2.0, a,  5);
            Assert.Equal(10.0, b,  5);
        }

        [Fact]
        public void Uncertainty_ReturnsZero_WhenPerfectFit()
        {

            var actual = new List<double> { 10, 11, 12 };
            var predicted = new List<double> { 10.0, 11.0, 12.0 };

            double result = StatisticsService.Uncertainty(actual, predicted);

            Assert.Equal(0.0,result ,5);
        }

        [Fact]
        public void Uncertainty_CalculatesCorrectly_ForKnownError()
        {
            var actual = new List<double> { 10, 11, 13 };
            var predicted = new List<double> { 10.0, 11.0, 12.0 };

            double expected = 1.1547;

            double result = StatisticsService.Uncertainty(actual, predicted);

            Assert.Equal(expected,result,5);
        }
    }
}
