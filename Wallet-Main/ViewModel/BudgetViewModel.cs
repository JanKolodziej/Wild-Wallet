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
        private List<BudgetWrapperDisplay> budgets;
        Wallet_lib.WalletService _service;
        public BudgetViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
            Load_Data();
        }
        public async Task Load_Data()
        {
            var data = await _service.Get_All_Budget_Wrapper();
            Budgets = new(data.Select(b => new BudgetWrapperDisplay { BudgetWrapper = b }));
        }
        [RelayCommand]
        public async Task Show_Add_Budget_PopUp(BudgetWrapperDisplay? budget)
        {
            var categories = await _service.Get_All_Expense_Categories_Async();
            AddBudgetPopUp pop = new(categories, _service, budget?.BudgetWrapper.Budget);
            var result = await Shell.Current.ShowPopupAsync(pop);

            if (result is Wallet_lib.Budget newBudget)
            {
                if (budget == null)
                {
                    await _service.Add_Budget(newBudget);
                }
                else
                {

                    var budgetToUpdate = Budgets.FirstOrDefault(b => b.BudgetWrapper.Budget.Id == budget.BudgetWrapper.Budget.Id);
                    if (budgetToUpdate != null)
                    {
                        budgetToUpdate.BudgetWrapper.Budget.CategoryId = newBudget.CategoryId;
                        budgetToUpdate.BudgetWrapper.Budget.Goal = newBudget.Goal;
                    }
                }
                    await Load_Data();
            }
        }

        public async Task Budget_Sugestion()
        {

        }
    }
}
