using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using Wallet_Main.Converters;
using Wallet_Main.Resources.Strings;

namespace Wallet_Main
{
    public partial class MonthlySummaryViewModel : ObservableObject
    {
        public List<Wallet_lib.Transactions> Transactions { get; set; }
        public decimal Balance { get; set; }
        public decimal Expenses { get; set; }
        public decimal Income { get; set; }
        public decimal ExpensesLastMonth { get; set; }
        public decimal IncomeLastMonth { get; set; }
        public string SummaryMonth { get; set; }
        public ObservableCollection<ISeries> Series { get; set; } = new();
        public bool AreThereExpenses { get; set; }
        public int SavingStreak { get; set; }
        public int AchievedBudgets { get; set; }
        public int FailedBudgets { get; set; }
        public List<Wallet_lib.BudgetWrapper> Budgets { get; set; }
        private readonly Wallet_lib.WalletService _service;

        //Trend properties
        public string IncomeTrendIcon { get; set; }
        public string ExpenseTrendIcon { get; set; }
        public string IncomeTrendText { get; set; }
        public string ExpenseTrendText { get; set; }
        public string IncomeTrendColor { get; set; }
        public string ExpenseTrendColor { get; set; }
        //********
        public string SelectedCategoryName { get; set; }
        public string SelectedCategoryIcon { get; set; }
        public decimal SelectedCategoryAmount { get; set; }
        public string BudgetSummary { get; set; }
        public bool ThereAreNotBudgets { get; set; }

        public bool IsCategorySelected { get; set; }
        // Zmienne sterujące środkiem koła
        [ObservableProperty]
        private string centerIcon = "💸";
        [ObservableProperty]
        private string centerTitle = AppResources.Expenses;

        [ObservableProperty]
        private decimal centerValue;

        public MonthlySummaryViewModel(List<Wallet_lib.Transactions> transactions, Wallet_lib.WalletService service, int streak)
        {
            _service = service;
            Transactions = transactions;
            Expenses = Math.Abs(transactions.Where(t => t.Amount < 0).Sum(t => t.Amount));
            Income = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            Balance = Income - Expenses;
            SummaryMonth = AppResources.SummaryMonth + " " + transactions.Last().Date.ToString("MMMM");
            CenterValue = Expenses;
            Create_Chart();
            SavingStreak = streak;
            Load_Data();



        }



        [RelayCommand]
        public async void Load_Data()
        {
            Budgets = await _service.Get_All_Budget_Wrapper(DateTime.Now.AddMonths(-1));
            ExpensesLastMonth = Math.Abs(await _service.Get_Expenses_Month_Async(DateTime.Now.AddMonths(-2)));
            IncomeLastMonth = await _service.Get_Income_Month_Async(DateTime.Now.AddMonths(-2));
            int numerator = 0;
            foreach (var budget in Budgets)
            {
                if (budget.IsInBudget)
                {
                    numerator++;
                }
            }
            BudgetSummary = $"{numerator}/{Budgets.Count}";
            ThereAreNotBudgets = !Budgets.Any();

            //Calculate trends
            decimal income_trend = Income - IncomeLastMonth;
            decimal expense_trend = Expenses - ExpensesLastMonth;
            decimal income_percentage = IncomeLastMonth != 0 ? (income_trend) / IncomeLastMonth * 100 : 0;
            decimal expense_percentage = ExpensesLastMonth != 0 ? (expense_trend) / ExpensesLastMonth * 100 : 0;

            if (IncomeLastMonth == 0 && Income > 0)
            {
                IncomeTrendIcon = "▲";
                IncomeTrendText = $"{CurrencyConverterHelper.Convert(income_trend)}";
                IncomeTrendColor = "#4CAF50";
            }
            else if (Income > IncomeLastMonth)
            {
                IncomeTrendIcon = "▲";
                IncomeTrendText = $"{income_percentage:F0}%  {CurrencyConverterHelper.Convert(income_trend)}";
                IncomeTrendColor = "#4CAF50";
            }
            else if (Income < IncomeLastMonth)
            {
                IncomeTrendIcon = "▼";
                IncomeTrendText = $"{income_percentage:F0}%  {CurrencyConverterHelper.Convert(income_trend)}"; ;
                IncomeTrendColor = "#F44336";
            }
            else
            {
                IncomeTrendIcon = "-";
                IncomeTrendText = $"{income_percentage:F0}%  {CurrencyConverterHelper.Convert(income_trend)}";
                IncomeTrendColor = "#9E9E9E";

            }

            if (ExpensesLastMonth == 0 && Expenses > 0)
            {
                ExpenseTrendIcon = "▲";
                ExpenseTrendText = $"{expense_trend:C0}";
                ExpenseTrendColor = "#F44336";
            }
            else if (Expenses > ExpensesLastMonth)
            {
                ExpenseTrendIcon = "▲";
                ExpenseTrendText = $"{expense_percentage:F0}%  {CurrencyConverterHelper.Convert(expense_trend)}";
                ExpenseTrendColor = "#F44336";
            }
            else if (Expenses < ExpensesLastMonth)
            {
                ExpenseTrendIcon = "▼";
                ExpenseTrendText = $"{expense_percentage:F0}%  {CurrencyConverterHelper.Convert(expense_trend)}"; ;
                ExpenseTrendColor = "#4CAF50";
            }
            else
            {
                ExpenseTrendIcon = "=";
                ExpenseTrendText = $"{expense_percentage:F0}%  {CurrencyConverterHelper.Convert(expense_trend)}";
                ExpenseTrendColor = "#9E9E9E";

            }

        }
        [RelayCommand]
        public async Task Close_Logic()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
        public void Create_Chart()
        {
            var data = Transactions.Where(t => t.Amount < 0)
                .GroupBy(t => t.Category.Id)
                .Select(g => new { Category = g.First().Category, Total = g.Sum(t => t.Amount) })
                .ToList();
            if (data.Count == 0)
            {
                AreThereExpenses = false;
                return;
            }
            AreThereExpenses = true;
            foreach (var item in data)
            {
                var serie = new PieSeries<decimal>
                {
                    Name = item.Category.Name,
                    Values = new decimal[] { Math.Abs(item.Total) },
                    Fill = new SolidColorPaint(SKColor.Parse(item.Category.Color)),
                    MaxRadialColumnWidth = 30,
                    CornerRadius = 5,
                    IsHoverable = false,

                };
                serie.DataPointerDown += (chart, points) =>
                {
                    foreach (var other in Series)
                    {
                        if (other is PieSeries<decimal> pieSeria)
                        {
                            pieSeria.Pushout = 0;
                        }
                    }
                    serie.Pushout = 30;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {

                        CenterTitle = serie.Name;
                        CenterValue = Math.Abs(item.Total);
                        CenterIcon = item.Category.Icon;
                    });
                };
                Series.Add(serie);
            }


        }
    }
}

