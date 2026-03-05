using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wallet_Main
{
    public partial class ScheduledViewModel:ObservableObject
    {
        private readonly WalletService _service;
        [ObservableProperty]
        private ObservableCollection<GroupedAutomatedTransactions> automatedTransactionsList = new();
        public ScheduledViewModel(WalletService service)
        {
            _service = service;
            Load_Data();
        }
        public async Task Load_Data()
        {
            var data = await _service.Get_All_Automated_Transactions_Async();

            var groups = data.GroupBy(t => t.Frequency).Select(t => new GroupedAutomatedTransactions(t.Key, t.ToList())).
                OrderByDescending(t=>t.Frequency).ToList();
            AutomatedTransactionsList = new ObservableCollection<GroupedAutomatedTransactions>(groups);
        }

        [RelayCommand]
        private async Task Delete_Transaction(AutomatedTransaction transaction)
        {
            await _service.Delete_Automated_Transaction(transaction);
            GroupedAutomatedTransactions group = AutomatedTransactionsList.First(g => g.Contains(transaction));
            group.Remove(transaction);
            if (group.Count() == 0)
            {
                AutomatedTransactionsList.Remove(group);
            }

        }
        [RelayCommand]
        private async Task Edit_Transaction(AutomatedTransaction transaction)
        {
            Dictionary<string, object> info = new();
            info.Add("AutomatedTransactionToEdit", transaction);
            if (transaction.Amount < 0)
            {
                info.Add("NewTransaction", "Expense");
            }
            else
            {
                info.Add("NewTransaction", "Income");
            }
            info.Add("IsAutomatedTransaction", true);
            await Shell.Current.GoToAsync(nameof(AddTransactionPage), info);
        }
    }
}
