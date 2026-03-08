using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Wallet_lib;

namespace Wallet_Main
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseLiveCharts()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Wallet_db.db");
            builder.Services.AddDbContext<Wallet_lib.WalletContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
            System.Diagnostics.Debug.WriteLine($"MOJA BAZA: {dbPath}");



#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<Wallet_lib.WalletService>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<AddTransactionViewModel>();
            builder.Services.AddTransient<StatisticPage>();
            builder.Services.AddTransient<StatisticViewModel>();
            builder.Services.AddTransient<ScheduledViewModel>();
            builder.Services.AddTransient<ScheduledPage>();
            builder.Services.AddTransient<BudgetViewModel>();
            builder.Services.AddTransient<BudgetPage>();

            //builder.Services.AddTransientPopup<CategoryPopUp, CategoryPopUpViewModel>();
            //builder.Services.AddTransientPopup<AddCategoryPopUp, AddCategoryViewModel>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Wallet_lib.WalletContext>();
                //db.Database.EnsureDeleted();
                db.Database.EnsureCreated();


                if (!db.Categories.Any())
                {
                    db.Categories.AddRange(new List<Wallet_lib.Categories>
                    {
                        new() { Name=Resources.Strings.CategoryNames.ShoppingCategory,    Type="Expense", Icon="🛒", Color="#FFA726" },
                        new() { Name=Resources.Strings.CategoryNames.EatingOutCategory,  Type="Expense", Icon="🍔", Color="#EF5350" },
                        new() { Name=Resources.Strings.CategoryNames.HouseCategory,       Type="Expense", Icon="🏠", Color="#42A5F5" },
                        new() { Name=Resources.Strings.CategoryNames.TransportCategory, Type="Expense", Icon="🚗", Color="#7E57C2" },
                        new() { Name=Resources.Strings.CategoryNames.SportCategory, Type="Expense", Icon="🏀", Color="#7E5B32" }, //change
                        new() { Name=Resources.Strings.CategoryNames.HobbyCategory, Type="Expense", Icon="🎮", Color="#7F57A2" },//change
                        new() { Name=Resources.Strings.CategoryNames.EntertainmentCategory,  Type="Expense", Icon="🎬", Color="#EC407A" },
                        new() { Name=Resources.Strings.CategoryNames.SubscrbtionCategory,Type="Expense", Icon="📱", Color="#FF7043" },
                        new() { Name=Resources.Strings.CategoryNames.GiftsCategory,  Type="Expense",  Icon="🎁", Color="#AB47BC" },
                        new() { Name=Resources.Strings.CategoryNames.OtherCategory,      Type="Expense", Icon="📦", Color="#BDBDBD" },

                        new() { Name=Resources.Strings.CategoryNames.PaycheckCategory,   Type="Income",  Icon="💵 ", Color="#66BB6A" },
                        new() { Name=Resources.Strings.CategoryNames.BisnesCategory,    Type="Income",  Icon="📈", Color="#9CCC65" },
                        new() { Name=Resources.Strings.CategoryNames.GiftsCategory,  Type="Income",  Icon="🎁", Color="#AB47BC" },
                        new() { Name=Resources.Strings.CategoryNames.InterestCategory,   Type="Income",  Icon="🏦", Color="#29B6F6" },
                        new() { Name=Resources.Strings.CategoryNames.OtherCategory,   Type="Income",  Icon="💸", Color="#29B6F6" }//change


                    });
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("✅ Pomyślnie dodano dane startowe kategorii do bazy");
                }
                if (!db.Accounts.Any())
                {
                    db.Accounts.AddRange(new List<Wallet_lib.Accounts>
                    {
                        new Wallet_lib.Accounts { Id =1,Name="Konto Główne" },
                        new Wallet_lib.Accounts { Id =2,Name="Konto Oszczędnościowe" }

                    });
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("✅ Pomyślnie dodano dane startowe kont do bazy");
                }
                if(!db.AutomatedTransactions.Any())
                {
                    db.AutomatedTransactions.Add(new() { Id=1, Amount=1000,NextTime=DateTime.Now,CategoryId=10,Frequency=FrequencyOnceA.Month, AccountId=2,Name="Wypłata" });
                    db.SaveChanges();
                }
                if (!db.Transactions.Any())
                {
                    db.Transactions.AddRange(new List<Wallet_lib.Transactions>
                    {
                        new Wallet_lib.Transactions { Id=1, Amount = -50.50m,Date=DateTime.Now,CategoryId=1},
                        new Wallet_lib.Transactions { Id=2, Amount = 300.00m,Date=DateTime.Now, CategoryId=8 },
                        new Wallet_lib.Transactions { Id=3, Amount = 45.00m,Date=DateTime.Now,CategoryId=9  }
                    });

                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("✅ Pomyślnie dodano dane startowe tranzakcji do bazy");
                }

                var count = db.Transactions.Count();
                System.Diagnostics.Debug.WriteLine($"Aktualna liczba rekordów w bazie urządzenia: {count}");
                if(!db.Budgets.Any())
                {
                    db.Budgets.Add(new() { Id = 1, CategoryId = 1, Goal = 500 });
                    db.Budgets.Add(new() { Id = 2, CategoryId = 2, Goal = 300 });
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("✅ Pomyślnie dodano dane startowe budżetów do bazy");
                }

            }

            return app;
        }
    }
}
