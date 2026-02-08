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
        public ObservableCollection<Wallet_lib.Accounts> Account_list { get; set; } = new();
        public ObservableCollection<Transactions> Transactions_list { get; set; } = new();
        public ObservableCollection<Categories> Categories_list { get; set; } = new();

        


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
            Categories_list = new ObservableCollection<Categories>(cat);
            var ac = await _service.Get_Accounts_Async();
            Account_list = new ObservableCollection<Wallet_lib.Accounts>(ac);

            var data = await _service.Get_Transaction_month(date);
            Transactions_list = new ObservableCollection<Wallet_lib.Transactions>(data);
            Income = await _service.Get_Income_Async(date);
            Expenses = await _service.Get_Expenses_Async(date);
            Balance = await _service.Get_Balance_Async();
        }
        [RelayCommand]
        public async Task Refresh_Data()
        {
            var data = await _service.Get_Transaction_month(DateTime.Now);
            Transactions_list.Clear();
           foreach(var d in data)
           {
                Transactions_list.Add(d);
           }
        }
          
        [RelayCommand]
        private async Task NewTransaction()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
            
        }
    }
}
