using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Wallet_Main.ViewModel;

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

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Wallet_lib.WalletContext>();
                //db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                if (!db.Transactions.Any())
                {
                    db.Transactions.AddRange(new List<Wallet_lib.Transactions>
                    {
                        new Wallet_lib.Transactions { Id=1, Amount = -50.50m,Date=DateTime.Now },
                        new Wallet_lib.Transactions { Id=2, Amount = 300.00m,Date=DateTime.Now  },
                        new Wallet_lib.Transactions { Id=3, Amount = 45.00m,Date=DateTime.Now  }
                    });

                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("✅ Pomyślnie dodano dane startowe tranzakcji do bazy");
                }
                if (!db.Categories.Any())
                {
                    db.Categories.AddRange(new List<Wallet_lib.Categories>
                    {
                        new Wallet_lib.Categories { Id =1,Name="Zakupy", Type="Expense" },
                        new Wallet_lib.Categories { Id =2,Name="Transport" , Type="Expense"},
                        new Wallet_lib.Categories { Id =3,Name="Rachunki", Type="Expense" },
                        new Wallet_lib.Categories { Id =4,Name="Wypłata", Type="Income" },
                        new Wallet_lib.Categories { Id =5,Name="Odsetki" , Type="Income"},
                        new Wallet_lib.Categories { Id =6,Name="Przychody finansowe", Type="Income" }


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

                var count = db.Transactions.Count();
                System.Diagnostics.Debug.WriteLine($"Aktualna liczba rekordów w bazie urządzenia: {count}");


            }

            return app;
        }
    }
}
