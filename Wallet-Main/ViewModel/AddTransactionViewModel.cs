using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using Wallet_lib;

namespace Wallet_Main
{
    [QueryProperty(nameof(TransactionToEdit), "TransactionToEdit")]
    [QueryProperty(nameof(IsAutomatedTransaction), "IsAutomatedTransaction")]
    [QueryProperty(nameof(AutomatedTransaction1), "AutomatedTransactionToEdit")]
    [QueryProperty(nameof(TypeOfTransaction), "NewTransaction")]
    public partial class AddTransactionViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNewTransaction))]
        private Transactions? transactionToEdit;
        public bool IsNewTransaction => TransactionToEdit == null;
        [ObservableProperty]
        private string? typeOfTransaction;

        [ObservableProperty]
        private DateTime date;
        [ObservableProperty]
        private string? title;
        [ObservableProperty]
        private int? accountId;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAmountValid))]
        private string amount;
        [ObservableProperty]
        private Categories? selectedCategory;
        //Automated Transactions
        [ObservableProperty]
        private bool isAutomatedTransaction;
        [ObservableProperty]
        private int frequency;

        [ObservableProperty]
        private AutomatedTransaction? automatedTransaction1;


        public bool IsAmountValid => decimal.TryParse(Amount, out _);
        public ObservableCollection<Categories> Categories { get; set; } = new();
        public ObservableCollection<Wallet_lib.Accounts> Accounts { get; set; } = new();

        public List<string> TypesFrequency { get; } =
        [
            Resources.Strings.AppResources.Daily,
            Resources.Strings.AppResources.Weekly,
            Resources.Strings.AppResources.Monthly,
            Resources.Strings.AppResources.Yearly
        ];
        public string IsoCurencySymbol { get; } = Preferences.Default.Get("UserCurrencyIso", new RegionInfo(CultureInfo.CurrentCulture.Name).ISOCurrencySymbol);


        private readonly Wallet_lib.WalletService _service;
        public AddTransactionViewModel(Wallet_lib.WalletService service)
        {
            Date = DateTime.Now;
            _service = service;
            Amount = string.Empty;
            IsAutomatedTransaction = false;
            Load_Data();

        }
        [RelayCommand]
        public async Task Show_Category_PopUp()
        {
            CategoryPopUp pop = new(Categories.ToList());
            var result = await Shell.Current.ShowPopupAsync(pop);

            if (result is Wallet_lib.Categories selected)
            {
                SelectedCategory = selected;
                if (!Categories.Contains(SelectedCategory))
                {
                    Categories.Add(SelectedCategory);
                    _service.Add_Category(SelectedCategory);
                }

            }
            else if (result == null)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Name == Resources.Strings.CategoryNames.OtherCategory && c.Type == Categories.Last().Type);
            }
        }

        partial void OnTransactionToEditChanged(Transactions? value)
        {
            if (value != null)
            {
                Date = value.Date;
                Title = value.Title;
                SelectedCategory = value.Category;
                AccountId = value.AccountId;
                Amount = Math.Abs(value.Amount).ToString();


            }
        }
        partial void OnAutomatedTransaction1Changed(AutomatedTransaction? value)
        {
            if (value != null)
            {
                Date = value.NextTime;
                Title = value.Name;
                SelectedCategory = value.Category;
                AccountId = value.AccountId;
                Amount = Math.Abs(value.Amount).ToString();
                Frequency = (int)value.Frequency;
            }
        }
        async partial void OnTypeOfTransactionChanged(string? value)
        {
            if (value != null)
            {
                if (value == "Expense")
                {
                    var cat = await _service.Get_All_Expense_Categories_Async();
                    Categories.Clear();
                    foreach (var category in cat)
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
                if (TransactionToEdit == null && AutomatedTransaction1 == null)
                {
                    await Show_Category_PopUp();
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
            if (IsAmountValid && SelectedCategory != null)
            {
                decimal _amount = decimal.Parse(Amount);
                if (TypeOfTransaction == "Expense" && _amount > 0)
                {
                    _amount = -_amount;
                }

                if (TransactionToEdit == null && AutomatedTransaction1 == null)
                {
                    if (IsAutomatedTransaction)
                    {
                        AutomatedTransaction automatedTransaction = new()
                        {
                            Name = Title,
                            Amount = _amount,
                            NextTime = Date,
                            CategoryId = SelectedCategory.Id,
                            Frequency = (FrequencyOnceA)Frequency,
                            AccountId = AccountId,

                        };
                        await _service.Add_AutomatedTransaction(automatedTransaction);
                    }
                    else
                    {
                        _service.Add_Transaction(new(Date, _amount, Title, SelectedCategory.Id, AccountId));
                    }

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    if (IsAutomatedTransaction)
                    {
                        AutomatedTransaction1.AccountId = AccountId;
                        AutomatedTransaction1.CategoryId = SelectedCategory.Id;
                        if (AutomatedTransaction1.Frequency != (FrequencyOnceA)Frequency)
                        {
                            AutomatedTransaction1.Frequency = (FrequencyOnceA)Frequency;
                            AutomatedTransaction1.NextTime = Date;
                        }
                        AutomatedTransaction1.Amount = _amount;
                        AutomatedTransaction1.Name = Title;
                        await _service.Update_Automated_Transaction(AutomatedTransaction1);

                    }
                    else
                    {
                        TransactionToEdit.Id = TransactionToEdit.Id;
                        TransactionToEdit.AccountId = AccountId;
                        TransactionToEdit.CategoryId = SelectedCategory.Id;
                        TransactionToEdit.Amount = _amount;
                        TransactionToEdit.Date = Date;
                        TransactionToEdit.Title = Title;

                        await _service.Update_Transaction(TransactionToEdit);
                    }

                    await Shell.Current.GoToAsync("..");
                }

            }

        }

    }
}
