using CommunityToolkit.Maui.Views;
namespace Wallet_Main;

public partial class MonthSummaryPopUp : Popup
{
    public decimal SpentThisMonth { get; set; }
    public decimal SpentLastMonth { get; set; }
    public decimal BalanceThisMonth { get; set; }
    public decimal BalanceLastMonth { get; set; }
    public decimal SavedLastMonth => BalanceLastMonth - SpentLastMonth;
    public decimal SavedThisMonth => BalanceThisMonth - SpentThisMonth;
    public string BalanceText { get; set; } = string.Empty;

    private readonly Wallet_lib.WalletService _service;
    private double currentY;
    public string MainText { get; set; } = string.Empty;


    public MonthSummaryPopUp(Wallet_lib.WalletService service)
    {
        InitializeComponent();
        BindingContext = this;
        _service = service;
        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        Size = new Size(mainDisplayInfo.Width / mainDisplayInfo.Density, 450);
    }

    public async Task Load_Data()
    {
        BalanceThisMonth = await _service.Get_Balance_Month_Async(DateTime.Now);
        BalanceLastMonth = await _service.Get_Balance_Month_Async(DateTime.Now.AddMonths(-1));

        SpentThisMonth = await _service.Get_Expenses_Month_Async(DateTime.Now);
        SpentLastMonth = await _service.Get_Expenses_Month_Async(DateTime.Now.AddMonths(-1));

    }
    public void Text_Logic()
    {
        if (BalanceThisMonth > SpentThisMonth)
        {
        }
    }




    public async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                if (e.TotalY > 0)
                {
                    MainPop.TranslationY = e.TotalY;
                    currentY = e.TotalY;

                }
                break;
            case GestureStatus.Completed:
                if (currentY > 200)
                {
                    //await CloseWithAnimation();
                }
                else
                {
                    await MainPop.TranslateTo(0, 0, 200, Easing.CubicOut);
                }
                currentY = 0;
                break;
        }
    }
}