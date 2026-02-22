using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;

namespace Wallet_Main
{
    public partial class MonthlyStatService: ObservableObject
    {
        public MonthlyStat MonthlyStat { get; set; }
        [ObservableProperty]
        private bool isExpanded;

        public MonthlyStatService(DateTime date)
        {
            MonthlyStat= new MonthlyStat(date);
            IsExpanded = false;
        }
    }
}
