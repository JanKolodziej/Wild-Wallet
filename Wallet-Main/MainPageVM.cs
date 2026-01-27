using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Wallet_Main
{
    public partial class MainPageVM:ObservableObject
    {
        [ObservableProperty]
        private decimal income;
        [ObservableProperty]
        private decimal balance;
        [ObservableProperty]
        private decimal expenses;

        private readonly WalletService _service;
        public ObservableCollection<Wallet_lib.Accounts> Account_list { get; set; }
        public ObservableCollection<Transactions> Transactions_list { get; set; }
        public ObservableCollection<Categories> Categories_list { get; set; }

        


        public MainPageVM(WalletService service)
        {  
            _service = service;
            Transactions_list = new ObservableCollection<Transactions>();
            Categories_list = new ObservableCollection<Categories>();
            Account_list = new ObservableCollection<Wallet_lib.Accounts>();
            Import_Accounts();
            Import_Categories();
            Import_Transactions();
        }
        public async void Import_Transactions()
        {
            Transactions_list.Clear();
            var data = await _service.GetAllTransactionsAsync();
            Transactions_list = new ObservableCollection<Transactions>(data);
        }
        public async void Import_Categories()
        {
            Categories_list.Clear();
            var data = await _service.GetAllCategoriesAsync();
            Categories_list = new ObservableCollection<Categories>(data);
        }
        public async void Import_Accounts()
        {
            Account_list.Clear();
            var data = await _service.GetAccountsAsync();
            Account_list = new ObservableCollection<Wallet_lib.Accounts>(data);
        }
    }
}
