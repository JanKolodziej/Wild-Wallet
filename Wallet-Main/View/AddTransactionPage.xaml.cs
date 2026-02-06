namespace Wallet_Main;

public partial class AddTransactionPage : ContentPage
{
	public AddTransactionPage(AddTransactionViewModel context)
	{
		InitializeComponent();
		BindingContext = context;
	}


}