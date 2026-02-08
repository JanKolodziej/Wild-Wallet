namespace Wallet_Main
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(BindingContext is MainPageViewModel vm)
            {
                vm.Refresh_DataCommand.Execute(null);
            }
        }

    }
}
