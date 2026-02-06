using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Wallet_Main
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Wallet_db.db");
            builder.Services.AddDbContext<Wallet_lib.WalletContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
                
            

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<Wallet_lib.WalletService>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<AddTransactionViewModel>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Wallet_lib.WalletContext>();
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
                    System.Diagnostics.Debug.WriteLine("✅ Pomyślnie dodano dane startowe do bazy");
                }

                var count = db.Transactions.Count();
                System.Diagnostics.Debug.WriteLine($"Aktualna liczba rekordów w bazie urządzenia: {count}");
            }
            return app;
        }
    }
}
