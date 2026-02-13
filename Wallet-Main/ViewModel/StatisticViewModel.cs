using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
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
            //LineSeries<DateTimePoint> series = new();
            ObservableCollection<double> values = new();
            foreach (var monthStat in MonthlyStatList)
            {
                amount +=(double) monthStat.Balance;
                //DateTime date = new(monthStat.Year, monthStat.Month,15);
                //DateTimePoint element = new(date, amount);
                //values.Add(element);
                values.Add(amount);

            }
            double a, b;

            (a, b) = Wallet_lib.StatisticsService.Line_Fit(values.Select(v => (decimal)v).ToList());
            int n = MonthlyStatList.Count();
            List<int> xValues = new();
            for (int i = 0; i < n+3; i++)
            {
                xValues.Add(i);
            }
            double[] ForecastY = xValues.Select(x => x * a + b).ToArray();
            //series.Values = values;
            SeriesList = new ISeries[]
            {
                new ScatterSeries<double>
                {
                    Values = values,
                    Name = "Wydatki miesięczne",
                    Fill = new SolidColorPaint(SKColors.White),
                    MinGeometrySize = 10
                },
                 new LineSeries<double>
                {
                    Values = ForecastY,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.CadetBlue, 3) { PathEffect = new DashEffect(new float[] { 10, 5 }) },
                    GeometrySize = 10,
                    Name = "Prognoza"
                }

            };
            XAxes = new Axis[]
            {

                
            };
        }

       
        

    }
}
