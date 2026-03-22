using CommunityToolkit.Mvvm.ComponentModel;
using Wallet_lib;

namespace Wallet_Main
{
    public partial class MonthlyStatService : ObservableObject
    {
        public MonthlyStat MonthlyStat { get; set; }
        [ObservableProperty]
        private bool isExpanded;

        public MonthlyStatService(DateTime date)
        {
            MonthlyStat = new MonthlyStat(date);
            IsExpanded = false;
        }
    }
}
