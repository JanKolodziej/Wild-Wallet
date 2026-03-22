namespace Wallet_Main;

public partial class ScheduledPage : ContentPage
{
    public ScheduledPage(ScheduledViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}