using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;

namespace Wallet_Main
{
    public partial class AddTransactionViewModel:ObservableObject
    {
        [ObservableProperty]
        private DateTime date;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private int categoryId;
        [ObservableProperty]
        private int accountId;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(isAmountValid))]
        private string amount;

        public bool isAmountValid => decimal.TryParse(Amount, out _);
        



        private readonly Wallet_lib.WalletService _service;
        public AddTransactionViewModel(Wallet_lib.WalletService service)
        {
            Date = DateTime.Now;
            _service = service;
        }

        [RelayCommand]
        public async Task Add()
        {
            if(isAmountValid)
            {
                _service.Add_Transaction(new(Date, decimal.Parse(Amount), Name, CategoryId, AccountId));
                await Shell.Current.GoToAsync("..");
            }
            
        }
       
    }
}
