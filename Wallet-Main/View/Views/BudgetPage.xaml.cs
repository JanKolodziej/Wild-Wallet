namespace Wallet_Main;

public partial class BudgetPage : ContentPage
{
    public BudgetPage(BudgetViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}