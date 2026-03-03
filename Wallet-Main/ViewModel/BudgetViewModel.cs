using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;

namespace Wallet_Main
{
    public partial class BudgetViewModel :ObservableObject
    {
        [ObservableProperty]
        private List<Wallet_lib.BudgetWrapper> budgets;
        Wallet_lib.WalletService _service;
        public Rect ExpectedProgressBounds
        {
            get
            {
                int currentDay = DateTime.Today.Day;
                int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);

                return new Rect((double)currentDay / daysInMonth, 0, 2, 1);
            }
        }
        public BudgetViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
            Load_Data();
        }
        public async Task Load_Data()
        {
            Budgets = await _service.Get_All_Budget_Wrapper();
        }
        [RelayCommand]
        public async Task Show_Add_Budget_PopUp(Wallet_lib.BudgetWrapper? budget)
        {
            var categories = await _service.Get_All_Expense_Categories_Async();
            AddBudgetPopUp pop = new(categories, _service, budget?.Budget);
            var result = await Shell.Current.ShowPopupAsync(pop);

            if (result is Wallet_lib.Budget newBudget)
            {
                if (budget == null)
                {
                    await _service.Add_Budget(newBudget);
                }
                else
                {

                    var budgetToUpdate = Budgets.FirstOrDefault(b => b.Budget.Id == budget.Budget.Id);
                    if (budgetToUpdate != null)
                    {
                        budgetToUpdate.Budget.CategoryId = newBudget.CategoryId;
                        budgetToUpdate.Budget.Goal = newBudget.Goal;
                    }
                }
                    await Load_Data();
            }
        }
    }
}
