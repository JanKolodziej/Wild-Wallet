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
            Import_Data();
            foreach(Transactions transaction in Transactions_list)
            {
                Calculate(transaction);
            }

            Get_Transaction_In_Month(DateTime.Now);
        }

        public async void Get_Transaction_In_Month(DateTime date)
        {
            var data = await _service.Get_Transaction_month(date);
            Transactions_list = new ObservableCollection<Wallet_lib.Transactions>(data);

        }
        public async void Import_Data()
        {
            var ac = await _service.Get_Accounts_Async();
            Account_list = new ObservableCollection<Wallet_lib.Accounts>(ac);
            var cat = await _service.Get_All_Categories_Async();
            Categories_list = new ObservableCollection<Categories>(cat);
            var tr = await _service.Get_All_Transactions_Async();
            Transactions_list = new ObservableCollection<Transactions>(tr);
        }

        public void Calculate(Transactions transaction)
        {
            if (transaction.Amount >= 0)
            {
                Income += transaction.Amount;
            }
            else if (transaction.Amount < 0)
            {
                Expenses += transaction.Amount;
            }
            Balance += transaction.Amount;
        }
        [RelayCommand]
        private async Task NewTransaction()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
            Transactions transaction = new Transactions
            {
                Amount = 10,
                Date=DateTime.Now
            };
            _service.Add_Transaction(transaction);
            Transactions_list.Add(transaction);
            Calculate(transaction);
        }
    }
}
