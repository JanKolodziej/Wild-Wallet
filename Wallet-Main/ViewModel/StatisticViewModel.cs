using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using System.Collections.ObjectModel;
using Wallet_lib;

namespace Wallet_Main
{
    public partial class StatisticViewModel : ObservableObject
    {
        private readonly Wallet_lib.WalletService _service;
        public ObservableCollection<Wallet_lib.Transactions> TransactionsList { get; set; } = new();
        public ObservableCollection<MonthlyStatService> MonthlyStatList { get; set; } = new();

        [ObservableProperty]
        private IEnumerable<ISeries> seriesList = Array.Empty<ISeries>();
        [ObservableProperty]
        private Axis[] xAxes = Array.Empty<Axis>();
        [ObservableProperty]
        private Axis[] yAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private bool isNotEnoughData = true;

        [ObservableProperty]
        private double dataCollectionProgress;

        [ObservableProperty]
        private string daysLeftText = string.Empty;


        public StatisticViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
            Load_Data();
            Do_I_Load_the_chart();
            if (!IsNotEnoughData)
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
                if (MonthlyStatList.Any())
                {
                    if (MonthlyStatList.Last().MonthlyStat.Month != transaction.Date.Month)
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
                    MonthlyStatList.Last().MonthlyStat.Income += transaction.Amount;
                }
                else
                {
                    MonthlyStatList.Last().MonthlyStat.Expenses += transaction.Amount;
                }
                CategoriesStat? k = MonthlyStatList.Last().MonthlyStat.CategoriesStats.FirstOrDefault(c => c.Category == transaction.Category);
                if(k == null)
                {
                    MonthlyStatList.Last().MonthlyStat.CategoriesStats.Add(new(transaction.Category, transaction.Amount));
                }
                else
                {
                    k.Balance += transaction.Amount;
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
                if (days > requiredDays)
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
            ObservableCollection<double> values = new();
            ObservableCollection<string> labels = new();
            foreach (var monthStat in MonthlyStatList)
            {
                amount += (double)monthStat.MonthlyStat.Balance;
                labels.Add($"{monthStat.MonthlyStat.Month}/{monthStat.MonthlyStat.Year}");
                values.Add(amount);

            }
            DateTime lastDate = new(MonthlyStatList.Last().MonthlyStat.Year, MonthlyStatList.Last().MonthlyStat.Month, 1);
            for (int i = 0; i < 3; i++)
            {
                lastDate = lastDate.AddMonths(1);
                labels.Add($"{lastDate.Month}/{lastDate.Year}");
            }
            labels.Add($"{MonthlyStatList.Last().MonthlyStat.Month + 1}/{MonthlyStatList.Last().MonthlyStat.Year}");
            double a, b;

            (a, b) = Wallet_lib.StatisticsService.Line_Fit(values.ToList());
            int n = MonthlyStatList.Count();
            List<int> xValues = new();
            for (int i = 0; i < n + 3; i++)
            {
                xValues.Add(i);
            }
            List<double> ForecastY = xValues.Select(x => x * a + b).ToList();
            double u = StatisticsService.Uncertainty(values.ToList(), ForecastY);
            List<double> upperBound = new();
            List<double> lowerBound = new();

            int lastRealIndex = values.Count;

            for (int i = 0; i < n + 3; i++)
            {


                if (i < lastRealIndex)
                {
                    upperBound.Add(ForecastY[i]);
                    lowerBound.Add(ForecastY[i]);
                }
                else
                {
                    int monthsIntoFuture = i - lastRealIndex;

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

        [RelayCommand]
        private void Expand_Month(MonthlyStatService monthlyStat)
        {
            if(monthlyStat != null)
            {
                monthlyStat.IsExpanded = !monthlyStat.IsExpanded;
            }
        }

    }
}
