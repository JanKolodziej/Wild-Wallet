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
        public ObservableCollection<Transactions> TransactionsList { get; set; } = new();
        public ObservableCollection<Categories> CategoriesList { get; set; } = new();
        public ObservableCollection<GroupedTransacions> GroupedTransactionsList { get; set; } = new();

        


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
        }
        [RelayCommand]
        public void Refresh_Data()
        {
            Import_Data(DateTime.Now);
        }
          
        [RelayCommand]
        private async Task NewTransaction()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
            
        }
    }
}
