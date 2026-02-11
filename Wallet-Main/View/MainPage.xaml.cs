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
            if (BindingContext is MainPageViewModel vm)
            {
                vm.Refresh_DataCommand.Execute(null);
            }
        }
        private bool _isMenuOpen = false;

        private async void MainFab_Clicked(object sender, EventArgs e)
        {
            _isMenuOpen = !_isMenuOpen;

            if (_isMenuOpen)
            {
                
                await MainFab.RotateTo(45, 250, Easing.CubicOut);

                IncomeBtn.InputTransparent = false;
                ExpenseBtn.InputTransparent = false;

                
                _ = ExpenseBtn.TranslateTo(0, -70, 250, Easing.CubicOut);
                _ = ExpenseBtn.FadeTo(1, 250);
                _ = ExpenseBtn.ScaleTo(1, 250);

                _ = IncomeBtn.TranslateTo(0, -130, 250, Easing.CubicOut);
                _ = IncomeBtn.FadeTo(1, 250);
                _ = IncomeBtn.ScaleTo(1, 250);
            }
            else
            {
                await MainFab.RotateTo(0, 250, Easing.CubicIn);

                IncomeBtn.InputTransparent = true;
                ExpenseBtn.InputTransparent = true;

                _ = IncomeBtn.TranslateTo(0, 0, 200);
                _ = IncomeBtn.FadeTo(0, 200);
                _ = IncomeBtn.ScaleTo(0, 200);

                _ = ExpenseBtn.TranslateTo(0, 0, 200);
                _ = ExpenseBtn.FadeTo(0, 200);
                _ = ExpenseBtn.ScaleTo(0, 200);
            }
        }

    }
}
