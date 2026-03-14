using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using System.Threading.Tasks;
using Wallet_lib;

namespace Wallet_Main;

public partial class MonthlySummaryPage : ContentPage
{
	public MonthlySummaryPage(MonthlySummaryViewModel vm)
	{

        InitializeComponent();
		BindingContext = vm;
	}
    private async void CloseModal_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void Add_Budget_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PopModalAsync(); 
        await Shell.Current.GoToAsync("//budgets");
    }
}