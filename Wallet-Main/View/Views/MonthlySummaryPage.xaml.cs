using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
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
}