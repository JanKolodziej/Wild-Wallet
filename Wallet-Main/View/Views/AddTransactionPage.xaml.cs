namespace Wallet_Main;

public partial class AddTransactionPage : ContentPage
{
    private readonly AddTransactionViewModel _vm;
    public AddTransactionPage(AddTransactionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ToolbarItems.Clear();
        if(!_vm.IsNewTransaction)
        {
            ToolbarItems.Add(new ToolbarItem()
            {
                IconImageSource = ImageSource.FromFile("delete.png"),
                Command =_vm.DeleteCommand

            });
        }
        

    }


}