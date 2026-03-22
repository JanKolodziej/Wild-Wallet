using System.Collections.ObjectModel;

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
        public ObservableCollection<CategoriesStat> CategoriesStats { get; set; } = new();

        public MonthlyStat(DateTime date)
        {
            Month = date.Month;
            Year = date.Year;
            Month_mmmm = date.ToString("MMMM");
        }

    }
    public class CategoriesStat
    {
        public Categories Category { get; set; }
        public decimal Balance { get; set; }
        public CategoriesStat(Categories categories, decimal balance)
        {
            Category = categories;
            Balance = balance;
        }
    }
}
