namespace Wallet_lib
{
    public class BudgetWrapper
    {
        public Budget Budget { get; set; }
        public decimal Spent { get; set; }
        public double Percentage { get; set; }
        public double ExpectedPercentage { get; set; }
        private DateTime Date;
        public bool IsInBudget => Percentage <= 1.0;

        public Color ProgressColor
        {
            get
            {
                if (Percentage > 1.0)
                    return Colors.Red;
                if (Percentage > ExpectedPercentage)
                    return Colors.Orange;
                return Color.FromArgb("#00BFA5");
            }
        }
        public BudgetWrapper(Budget budget, decimal spent, DateTime date)
        {
            Budget = budget;
            Spent = Math.Abs(spent);
            Date = date;
            if (Budget.Goal != 0)
            {
                Percentage = (double)Spent / (double)Budget.Goal;
            }
            else
            {
                Percentage = 0;
            }
            if (Date.Month == DateTime.Now.Month && Date.Year == DateTime.Now.Year)
            {
                ExpectedPercentage = DateTime.Now.Day / (double)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            }
            else
            {
                ExpectedPercentage = 1.0;
            }


        }
    }
}
