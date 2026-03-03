using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_lib
{
    public class BudgetWrapper
    {
        public Budget Budget { get; set; }
        public decimal Spent { get; set; }
        public double Percentage { get; set; }
        public double ExpectedPercentage { get; set; }

        public Color ProgressColor
        {
            get
            {
                if (Percentage >= 1.0)
                    return Colors.Red;
                if (Percentage > ExpectedPercentage)
                    return Colors.Orange;
                return Color.FromArgb("#00BFA5");
            }
        }
        public BudgetWrapper(Budget budget, decimal spent)
        {
            Budget = budget;
            Spent = spent;
            if (Budget.Goal != 0)
            {
                Percentage = (double)(Math.Abs(Spent) / Budget.Goal);
            }
            else
            {
                Percentage = 0;
            }
            ExpectedPercentage = DateTime.Now.Day / (double)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        }
    }
}
