using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;
using Wallet_lib;
namespace Wallet_Main
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private decimal income;
        [ObservableProperty]
        private decimal balance;
        [ObservableProperty]
        private decimal expenses;
       
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasUpcomingTransactions))]
        private ObservableCollection<AutomatedTransaction> upcomingTransactionsList = new ObservableCollection<AutomatedTransaction>();
        public bool HasUpcomingTransactions => UpcomingTransactionsList.Any();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasBudgets))]
        private ObservableCollection<BudgetWrapperDisplay> budgetWrapperList = new ObservableCollection<BudgetWrapperDisplay>();

        public bool HasBudgets => BudgetWrapperList.Any();

        private readonly WalletService _service;
        [ObservableProperty]
        private ObservableCollection<GroupedTransactions> groupedTransactionsList = new ObservableCollection<GroupedTransactions>();
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ZeroTransactions))]
        private ObservableCollection<Transactions> transactionsList = new ObservableCollection<Transactions>();
        [ObservableProperty]
        private IEnumerable<ISeries> seriesList = Array.Empty<ISeries>();
        [ObservableProperty]
        private Axis[] xAxes = Array.Empty<Axis>();
        [ObservableProperty]
        private Axis[] yAxes = Array.Empty<Axis>();

        public bool ZeroTransactions => !TransactionsList.Any();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DateLabel))]
        private DateTime date;
        public string DateLabel => DateFormater(Date);

        private static string DateFormater(DateTime date)
        {
            if (date.Year == DateTime.Now.Year)
            {
                return date.ToString("MMMM");
            }
            else
            {
                return date.ToString("MMM yyyy");
            }
        }

        public MainPageViewModel(WalletService service)
        {
            _service = service;
            Date = DateTime.Now;
            Expenses = 0;
            Balance = 0;
            Income = 0;
        }

        public async Task Import_Data()
        {
            //var cat = await _service.Get_All_Categories_Async();
            //CategoriesList = new ObservableCollection<Categories>(cat);
            //var ac = await _service.Get_Accounts_Async();
            //AccountList = new ObservableCollection<Wallet_lib.Accounts>(ac);
            var data = await _service.Get_Transaction_month(Date);
            TransactionsList = new ObservableCollection<Transactions>(data);

            var groups = data.GroupBy(t => t.Date.Date).Select(t => new GroupedTransactions(t.Key, t.ToList())).
                OrderByDescending(t => t.GroupDate).ToList();
            GroupedTransactionsList = new ObservableCollection<GroupedTransactions>(groups);

            Income = await _service.Get_Income_Async(Date);
            Expenses = await _service.Get_Expenses_Async(Date);
            Balance = await _service.Get_Balance_To_Date(Date);
            var upcoming = await _service.Get_Close_Automated_Transactions();
            UpcomingTransactionsList = new ObservableCollection<AutomatedTransaction>(upcoming);

            var budgets = await _service.Get_All_Budget_Wrapper();
            BudgetWrapperList = new ObservableCollection<BudgetWrapperDisplay>(budgets.Select(b => new BudgetWrapperDisplay { BudgetWrapper = b }));

            await Create_chart();
        }
        [RelayCommand]
        public async Task Refresh_Data()
        {
            await _service.ProcessAutomatedTransactions();
            await Import_Data();
        }

        public async Task Create_chart()
        {
            if (!TransactionsList.Any()) return;
            double amount = Convert.ToDouble(await _service.Get_Balance_To_Date(Date));
            LineSeries<DateTimePoint> series = new();
            ObservableCollection<DateTimePoint> values = new();
            List<Transactions> transactionsChart = new(TransactionsList.ToList());
            DateTime firstDayOfMonth = new(Date.Year, Date.Month, 1);
            if (transactionsChart.Last().Date.Date != firstDayOfMonth)
            {
                values.Add(new(firstDayOfMonth, amount));
            }
            transactionsChart.Reverse();
            foreach (Transactions trans in transactionsChart)
            {
                amount += (double)trans.Amount;
                if (values.Any())
                {
                    if (values.Last().DateTime.Date == trans.Date.Date)
                    {
                        values.Last().Value = amount;
                        continue;
                    }
                }


                DateTimePoint element = new(trans.Date, amount);
                values.Add(element);

            }
            
            series.Values = values;
            SeriesList = new ISeries[]
            {
                series
            };
            XAxes = new Axis[]
            {

                new DateTimeAxis(TimeSpan.FromDays(1), date=> date.ToString("dd"))
                {
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SkiaSharp.SKColors.Gray,
                        StrokeThickness = 2
                    }
                }
            };
            YAxes = new Axis[]
            {

                new Axis
                {
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SkiaSharp.SKColors.Gray,
                        StrokeThickness = 2
                    }
                }
            };
        }
        /// <summary>
        /// Delates transaction and refreshes content on page
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task Delete_Transaction(Transactions transaction)
        {
            await _service.Delete_Transaction(transaction);
            GroupedTransactions group = GroupedTransactionsList.First(g => g.Contains(transaction));
            group.Remove(transaction);
            if (group.Count() == 0)
            {
                GroupedTransactionsList.Remove(group);
            }
            TransactionsList.Remove(transaction);
            var data = await _service.Get_Transaction_month(Date);
            TransactionsList = new ObservableCollection<Transactions>(data);
            Income = await _service.Get_Income_Async(Date);
            Expenses = await _service.Get_Expenses_Async(Date);
            Balance = await _service.Get_Balance_To_Date(Date);


            await Create_chart();
        }

        [RelayCommand]
        private async Task New_Income()
        {
            Dictionary<string, object> info = new();
            info.Add("NewTransaction", "Income");
            await Shell.Current.GoToAsync(nameof(AddTransactionPage), info);
        }

        [RelayCommand]
        private async Task New_Expense()
        {
            Dictionary<string, object> info = new();
            info.Add("NewTransaction", "Expense");
            await Shell.Current.GoToAsync(nameof(AddTransactionPage), info);
        }
        [RelayCommand]
        private async Task Edit_Transaction(Transactions transaction)
        {
            Dictionary<string, object> info = new();
            info.Add("TransactionToEdit", transaction);
            if (transaction.Amount < 0)
            {
                info.Add("NewTransaction", "Expense");
            }
            else
            {
                info.Add("NewTransaction", "Income");
            }
            await Shell.Current.GoToAsync(nameof(AddTransactionPage), info);
        }
        [RelayCommand]
        private async Task Previous_Month()
        {
            Date = Date.AddMonths(-1);
            await Import_Data();
        }
        [RelayCommand]
        private async Task Next_Month()
        {
            Date = Date.AddMonths(1);
            await Import_Data();
        }
    }
}
