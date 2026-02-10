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
        private string? title;
        [ObservableProperty]
        private int? categoryId;
        [ObservableProperty]
        private int? accountId;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(isAmountValid))]
        private string amount;

        public bool isAmountValid => decimal.TryParse(Amount, out _);
        public ObservableCollection<Categories> Categories { get; set; } = new();
        public ObservableCollection<Wallet_lib.Accounts> Accounts { get; set; } = new();



        private readonly Wallet_lib.WalletService _service;
        public AddTransactionViewModel(Wallet_lib.WalletService service)
        {
            Date = DateTime.Now;
            _service = service;
            Load_Data();
        }
        public async void Load_Data()
        {
            var cat = await _service.Get_All_Categories_Async();
            Categories = new ObservableCollection<Categories>(cat);
            var acc = await _service.Get_Accounts_Async();
            Accounts = new ObservableCollection<Wallet_lib.Accounts>(acc);
        }
        [RelayCommand]
        public async Task Add()
        {
            if(isAmountValid)
            {
                _service.Add_Transaction(new(Date, decimal.Parse(Amount), Title, CategoryId, AccountId));
                await Shell.Current.GoToAsync("..");
            }
            
        }
       
    }
}
