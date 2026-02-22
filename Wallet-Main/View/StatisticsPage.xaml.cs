

namespace Wallet_Main;

public partial class StatisticPage : ContentPage
{
    public StatisticPage(StatisticViewModel statisticViewModel)
    {
        InitializeComponent();
        BindingContext = statisticViewModel;
    }
}