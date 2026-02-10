using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
namespace Wallet_Main
{
    public partial class MainPageViewModel:ObservableObject
    {
        [ObservableProperty]
        private decimal income;
        [ObservableProperty]
        private decimal balance;
        [ObservableProperty]
        private decimal expenses;

        private readonly WalletService _service;
        public ObservableCollection<Wallet_lib.Accounts> AccountList { get; set; } = new();
        public ObservableCollection<Categories> CategoriesList { get; set; } = new();
        public ObservableCollection<GroupedTransacions> GroupedTransactionsList { get; set; } = new();
        public ObservableCollection<Transactions> TransactionsList { get; set; } = new();

        public ISeries[] SeriesList { get; set; } 
        public Axis[] XAxes { get; set; } 


        public MainPageViewModel(WalletService service)
        {  
            _service = service;

            Expenses = 0;
            Balance = 0;
            Income = 0;
            Import_Data(DateTime.Now);


        }

        public async void Import_Data(DateTime date)
        {
            var cat = await _service.Get_All_Categories_Async();
            CategoriesList = new ObservableCollection<Categories>(cat);
            var ac = await _service.Get_Accounts_Async();
            AccountList = new ObservableCollection<Wallet_lib.Accounts>(ac);
            var data = await _service.Get_Transaction_month(DateTime.Now);
            TransactionsList.Clear();
            foreach (var d in data)
            {
                TransactionsList.Add(d);
            }
            GroupedTransactionsList.Clear();
            var groups = data.GroupBy(t => t.Date.Date).Select(t => new GroupedTransacions(t.Key, t.ToList())).
                OrderByDescending(t => t.GroupDate).ToList();
            foreach (var g in groups)
            {
                GroupedTransactionsList.Add(g);
            }

            Income = await _service.Get_Income_Async(date);
            Expenses = await _service.Get_Expenses_Async(date);
            Balance = await _service.Get_Balance_Async();
            Create_chart(); 
        }
        [RelayCommand]
        public void Refresh_Data()
        {
            Import_Data(DateTime.Now);
        }
          
        [RelayCommand]
        private async Task New_Transaction()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
            
        }
        public void Create_chart()
        {
            double amount = 0;
            LineSeries<DateTimePoint> series = new();
            ObservableCollection<DateTimePoint> values = new();
            foreach(var trans in TransactionsList)
            {
                amount += (double)trans.Amount;
                if(values.Any())
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
            series.Values= values;
            SeriesList = new ISeries[]
            {
                series
            };
            XAxes = new Axis[]
            {
                new DateTimeAxis(TimeSpan.FromDays(1), date=> date.ToString("dd"))
            };
        }
    }
}
