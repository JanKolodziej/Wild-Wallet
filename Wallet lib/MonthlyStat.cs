namespace Wallet_lib
{
    public class MonthlyStat
    {
        public int Month { get; set; }
        public string Month_mmmm { get; set; }
        public int Year { get; set; }
        public decimal Expenses { get; set; }
        public decimal Income { get; set; }
        public decimal Balance => Income + Expenses;
        public MonthlyStat(DateTime date)
        {
            Month = date.Month;
            Year = date.Year;
            Month_mmmm = date.ToString("MMMM");
        }

    }
}
