using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Wallet_lib;

namespace Wallet_Main
{
    [QueryProperty(nameof(TransactionToEdit), "TransactionToEdit")]
    [QueryProperty(nameof(TypeOfTransaction), "NewTransaction")]

    public partial class AddTransactionViewModel : ObservableObject
    {
        [ObservableProperty]
        private Transactions? transactionToEdit;
        [ObservableProperty]
        private string typeOfTransaction;

        [ObservableProperty]
        private DateTime date;
        [ObservableProperty]
        private string? title;
        [ObservableProperty]
        private int? categoryId;
        [ObservableProperty]
        private int? accountId;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAmountValid))]
        private string amount;

        public bool IsAmountValid => decimal.TryParse(Amount, out _);
        public ObservableCollection<Categories> Categories { get; set; } = new();
        public ObservableCollection<Wallet_lib.Accounts> Accounts { get; set; } = new();



        private readonly Wallet_lib.WalletService _service;
        public AddTransactionViewModel(Wallet_lib.WalletService service)
        {
            Date = DateTime.Now;
            _service = service;
            Load_Data();
            
        }
        partial void OnTransactionToEditChanged(Transactions? value)
        {
            if (value != null)
            {
                Date = value.Date;
                Title = value.Title;
                CategoryId = value.CategoryId;
                AccountId = value.AccountId;
                Amount = Math.Abs(value.Amount).ToString();


            }
        }
        async partial void OnTypeOfTransactionChanged(string value)
        {
            if (value == "Expense")
            {
                var cat = await _service.Get_All_Expense_Categories_Async();
                Categories.Clear();
                foreach(var category in cat)
                {
                    Categories.Add(category);
                }
            }
            else if (value == "Income")
            {
                var cat = await _service.Get_All_Income_Categories_Async();
                Categories.Clear();
                foreach (var category in cat)
                {
                    Categories.Add(category);
                }
            }
        }
        public async void Load_Data()
        {


            var acc = await _service.Get_Accounts_Async();
            Accounts = new ObservableCollection<Wallet_lib.Accounts>(acc);
        }
        [RelayCommand]
        public async Task Add()
        {
            if (IsAmountValid)
            {
                decimal _amount = decimal.Parse(Amount);
                if (TypeOfTransaction == "Expense" && _amount > 0)
                {
                    _amount = -_amount;
                }

                if (TransactionToEdit == null)
                {

                    _service.Add_Transaction(new(Date, _amount, Title, CategoryId, AccountId));
                    await Shell.Current.GoToAsync("..");
                }
                else 
                {
                    TransactionToEdit.Id = TransactionToEdit.Id;
                    TransactionToEdit.AccountId = AccountId;
                    TransactionToEdit.CategoryId= CategoryId;
                    TransactionToEdit.Amount = _amount;
                    TransactionToEdit.Date=Date;
                    TransactionToEdit.Title = Title;

                    await _service.Update_Transaction(TransactionToEdit);
                    await Shell.Current.GoToAsync("..");
                }
                
            }

        }

    }
}
