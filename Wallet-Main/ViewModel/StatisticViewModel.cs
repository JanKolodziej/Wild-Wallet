using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_Main.ViewModel
{
    public partial class StatisticViewModel:ObservableObject
    {
        private readonly Wallet_lib.WalletService _service;
        public ObservableCollection<Wallet_lib.Transactions> TransactionsList =new();
        public ObservableCollection<Wallet_lib.MonthlyStat> MonthlyStatList =new();

        [ObservableProperty]
        private IEnumerable<ISeries> seriesList;
        [ObservableProperty]
        private  Axis[] xAxes;

        public StatisticViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
            Load_Data();
            Create_Chart();
        }

        public async void Load_Data()
        {
            var data = await _service.Get_All_Transactions_Async();
            TransactionsList.Clear();
            foreach (var transaction in data)
            {
                TransactionsList.Add(transaction);
                if(MonthlyStatList.Any())
                {
                    if(MonthlyStatList.Last().Month != transaction.Date.Month)
                    {
                        MonthlyStatList.Add(new(transaction.Date));
                    }    
                }
                else
                {
                    MonthlyStatList.Add(new(transaction.Date));

                }
                if (transaction.Amount > 0)
                {
                    MonthlyStatList.Last().Income += transaction.Amount;
                }
                else
                {
                    MonthlyStatList.Last().Expenses += transaction.Amount;
                }


            }

        }
        public void Create_Chart()
        {
            double amount = 0;
            LineSeries<DateTimePoint> series = new();
            ObservableCollection<DateTimePoint> values = new();
            foreach (var monthStat in MonthlyStatList)
            {
                amount +=(double) monthStat.Balance;
                DateTime date = new(monthStat.Year, monthStat.Month,15);
                DateTimePoint element = new(date, amount);
                values.Add(element);

            }
            series.Values = values;
            SeriesList = new ISeries[]
            {
                series
            };
            XAxes = new Axis[]
            {

                new DateTimeAxis(TimeSpan.FromDays(30), date=> date.ToString("MM yyyy"))
            };
        }


    }
}
