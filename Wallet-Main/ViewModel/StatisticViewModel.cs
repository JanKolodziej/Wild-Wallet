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
using System.Threading;
using System.Threading.Tasks;
using Wallet_lib;

namespace Wallet_Main.ViewModel
{
    public partial class StatisticViewModel:ObservableObject
    {
        private readonly Wallet_lib.WalletService _service;
        public ObservableCollection<Wallet_lib.Transactions> TransactionsList { get; set; } = new();
        public ObservableCollection<Wallet_lib.MonthlyStat> MonthlyStatList { get; set; } =new();

        [ObservableProperty]
        private IEnumerable<ISeries> seriesList;
        [ObservableProperty]
        private  Axis[] xAxes;
        [ObservableProperty]
        private Axis[] yAxes;

        [ObservableProperty]
        private bool isNotEnoughData=true;

        [ObservableProperty]
        private double chartOpacity = 1.0; 

        [ObservableProperty]
        private double dataCollectionProgress; 

        [ObservableProperty]
        private string daysLeftText;


        public StatisticViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
            Load_Data();
            Do_I_Load_the_chart();
            if(!IsNotEnoughData)
            {
                Create_Chart();
            }
            
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
        public void Do_I_Load_the_chart()
        {
            int n = MonthlyStatList.Count;
            if (n > 2)
            {
                IsNotEnoughData = false;
            }
            else
            {
                DateTime date = TransactionsList.First().Date.AddMonths(3);
                DateTime requiredDate = new(date.Year, date.Month, 1);
                int requiredDays = (requiredDate - TransactionsList.Last().Date).Days;
                int days = (requiredDate - DateTime.Now.Date).Days;
                if(days>requiredDays)
                {
                    DaysLeftText = "Dodaj tranzakcje w zaległych miesiącach aby odblokować";
                    DataCollectionProgress = 0.8;
                }
                DaysLeftText = $"Pozostało {days} dni do odblokowania tej zawartości";
                DataCollectionProgress = (requiredDays - days) / requiredDays;
            }
        }
        public void Create_Chart()
        {
            
            double amount = 0;
            //LineSeries<DateTimePoint> series = new();
            ObservableCollection<double> values = new();
            ObservableCollection<string> labels = new();
            foreach (var monthStat in MonthlyStatList)
            {
                amount +=(double) monthStat.Balance;
                labels.Add($"{monthStat.Month}/{monthStat.Year}");
                values.Add(amount);

            }
            DateTime lastDate = new(MonthlyStatList.Last().Year, MonthlyStatList.Last().Month, 1);
            for (int i = 0; i < 3; i++)
            {
                lastDate = lastDate.AddMonths(1);
                labels.Add($"{lastDate.Month}/{lastDate.Year}");
            }
            labels.Add($"{MonthlyStatList.Last().Month+1}/{MonthlyStatList.Last().Year}");
            double a, b;

            (a, b) = Wallet_lib.StatisticsService.Line_Fit(values.ToList());
            int n = MonthlyStatList.Count();
            List<int> xValues = new();
            for (int i = 0; i < n+3; i++)
            {
                xValues.Add(i);
            }
            List<double> ForecastY = xValues.Select(x => x * a + b).ToList();
            double u = StatisticsService.Uncertainty(values.ToList() , ForecastY);
            List<double> upperBound = new();
            List<double> lowerBound = new();

            // Indeks ostatniego prawdziwego punktu (np. jeśli masz 5 punktów, to indeks 4)
            int lastRealIndex = values.Count;

            for (int i = 0; i < n + 3; i++)
            {
                

                // LOGIKA ODCIĘCIA:
                if (i < lastRealIndex)
                {
                    // Przeszłość: "Zamykamy" wstęgę.
                    // Góra i Dół są równe linii trendu.
                    // Maska (Dół) idealnie przykryje Górę, więc nic nie będzie widać.
                    upperBound.Add(ForecastY[i]);
                    lowerBound.Add(ForecastY[i]);
                }
                else
                {
                    // Obliczamy ile miesięcy minęło od ostatniego prawdziwego punktu
                    int monthsIntoFuture = i - lastRealIndex;

                    // Mnożymy niepewność przez czas -> powstaje "stożek"
                    // (Możesz dodać +1, żeby startowało od razu szeroko, albo zostawić 0 żeby startowało z punktu)
                    double currentUncertainty = u * 2 * (1 + (monthsIntoFuture * 0.4));

                    upperBound.Add(ForecastY[i] + currentUncertainty);
                    lowerBound.Add(ForecastY[i] - currentUncertainty);
                }
            }

            SeriesList = new ISeries[]
            {

            new LineSeries<double> { 
                Values = lowerBound, 
                Fill  = new SolidColorPaint(SKColor.Parse("#273446")), 
                ZIndex=-1 ,
                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null,
            },
            
            new LineSeries<double>
            {
                Values = upperBound,
                Name = $"Niepewność \u00B1{u*2:N1}", 
                Fill = new SolidColorPaint(SKColors.CadetBlue.WithAlpha(40)),
                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null,
                ZIndex=-2
            },
                new LineSeries<double>
                {
                    Values = ForecastY,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.CadetBlue, 3) { PathEffect = new DashEffect(new float[] { 10, 5 }) },
                    GeometrySize = 10,
                    Name = "Prognoza",
                    
                },
                 new ScatterSeries<double>
                {
                    Values = values,
                    Name = "Wydatki miesięczne",
                    Fill = new SolidColorPaint(SKColors.White),
                    MinGeometrySize = 5,
                    ZIndex=3
                }

            };
            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsPaint = new SolidColorPaint(SKColors.WhiteSmoke),
                    NamePaint = new SolidColorPaint(SKColors.WhiteSmoke),
                    LabelsRotation = 45,
                    TextSize=10
                }
            };
            YAxes = new Axis[]
            {
                new Axis
                {
                    NamePaint = new SolidColorPaint(SKColors.WhiteSmoke),
                    LabelsPaint = new SolidColorPaint(SKColors.WhiteSmoke),
                    Labeler = value => value.ToString("C"),
                    TextSize=10,
                    SeparatorsPaint = new SolidColorPaint(SKColors.WhiteSmoke)
                    {
                        PathEffect = new DashEffect(new float[] { 5, 5 }),
                        ZIndex = 1
                    }
                }
            };
        }

       
        

    }
}
