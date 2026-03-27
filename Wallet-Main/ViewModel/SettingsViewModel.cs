using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Wallet_lib;

namespace Wallet_Main
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Currency selectedCurrency;
        [ObservableProperty]
        private ObservableCollection<Wallet_lib.Currency> currencies = new()
        {
            new("Auto","System Currency","Auto","🌍"),
            new Currency { Iso = "PLN", Name = "Polski Złoty", Flag = "🇵🇱",Code="zł"},
            new Currency { Iso = "EUR", Name = "Euro", Flag = "🇪🇺",Code="€" },
            new Currency { Iso = "USD", Name = "Dolar amerykański", Flag = "🇺🇸",Code="$"},
            new Currency { Iso = "GBP", Name = "Funt brytyjski", Flag = "🇬🇧",Code="£"},
            new Currency { Iso = "CHF", Name = "Frank szwajcarski", Flag = "🇨🇭",Code="CHF" },
            new Currency { Iso = "NOK", Name = "Korona norweska", Flag = "🇳🇴",Code="kr" }
        };

        private readonly Wallet_lib.WalletService _service;
        public SettingsViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
            Load_Currency();
        }
        public void Load_Currency()
        {
            if (Preferences.Default.ContainsKey("UserCurrencyCode"))
            {
                string symbolWaluty = Preferences.Default.Get("UserCurrencyCode", "");

                SelectedCurrency = Currencies?.First(c => c.Code == symbolWaluty);
            }
            else
            {
                SelectedCurrency = Currencies[0];
            }
        }
        partial void OnSelectedCurrencyChanged(Currency oldValue, Currency newvalue)
        {
            if (SelectedCurrency == Currencies[0])
            {
                Preferences.Default.Remove("UserCurrencyCode");
                Preferences.Default.Remove("UserCurrencyIso");
            }
            else
            {
                Preferences.Default.Set("UserCurrencyCode", SelectedCurrency.Code);
                Preferences.Default.Set("UserCurrencyIso", SelectedCurrency.Iso);
            }

#if ANDROID

            if (oldValue != null)
            {

                string text = $"{Resources.Strings.AppResources.RestartApp}";
                var pop2 = Toast.Make(text, ToastDuration.Long, 14);
                pop2.Show();
            }

#endif
        }

        [RelayCommand]
        public async Task Export_Transactions()
        {
            var transactions = await _service.Get_All_Transactions_Async();
            var to_export = new StringBuilder();
            to_export.AppendLine("Id;Date;Amount;Title;CategoryName;CategoryType;CategoryIcon;CategoryColor;AccountId");
            foreach (var transaction in transactions)
            {
                string title = string.Empty;
                if (transaction.Title != null)
                {
                    title = transaction.Title.Replace(';', ' ');
                    title = title.Replace("/n", " ");
                }


                to_export.AppendLine($"{transaction.Id};{transaction.Date.ToString("yyyy-MM-dd")};{transaction.Amount};" +
                    $"{title};{transaction.Category.Name};{transaction.Category.Type};{transaction.Category.Icon}" +
                    $";{transaction.Category.Color};{transaction.AccountId}");
            }
            string Title = $"Wild_Wallet_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
            string path = Path.Combine(FileSystem.CacheDirectory, $"{Title}.csv");
            File.WriteAllText(path, to_export.ToString());
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = Title,
                File = new ShareFile(path)
            });
        }
        [RelayCommand]
        public async Task Import()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "text/comma-separated-values", "text/csv" } },
                { DevicePlatform.iOS, new[] { "public.comma-separated-values" } },
                { DevicePlatform.WinUI, new[] { ".csv" } }
            });

            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = Resources.Strings.AppResources.ChooseFile,
                FileTypes = customFileType,
            });
            if (file == null) return;
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            string[] lines = content.Split('\n');
            List<Categories> list = await _service.Get_All_Categories_Async();
            int categoriesAdded = 0;
            int transactionsAdded = 0;
            for (int i = 1; i < lines.Length; i++)
            {

                string[] elements = lines[i].Split(";");
                if (elements.Length == 9)
                {
                    DateTime date = DateTime.ParseExact(elements[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    decimal amount = Convert.ToDecimal(elements[2]);
                    string title = elements[3];
                    string categoryName = elements[4];
                    string categoryType = elements[5];
                    string categoryicon = elements[6];
                    string categoryColor = elements[7];
                    int accountId = 1;
                    string account = elements[8].Trim();
                    if (account != string.Empty && account != null)
                    {
                        accountId = Convert.ToInt32(elements[8]);
                    }

                    var cat = list.FirstOrDefault(c => c.Icon == categoryicon
                        && c.Type == categoryType && c.Name == categoryName &&
                        categoryColor == c.Color);
                    transactionsAdded++;
                    if (cat != null)
                    {
                        _service.Add_Transaction(new(date, amount, title, cat.Id, accountId));
                    }
                    else
                    {
                        Categories category = new(categoryName, categoryType, categoryicon, categoryColor);
                        _service.Add_Category(category);
                        categoriesAdded++;
                        list.Add(category);
                        int id = list.First(c => c.Icon == categoryicon
                            && c.Type == categoryType && c.Name == categoryName &&
                            categoryColor == c.Color).Id;

                        _service.Add_Transaction(new(date, amount, title, id, accountId));

                    }
                }




            }
#if ANDROID

            if (categoriesAdded >= 1)
            {
                string text_cat = $"{Resources.Strings.AppResources.SuccessAdded1} {transactionsAdded} {Resources.Strings.AppResources.SuccessAdded2} {Resources.Strings.AppResources.and} {categoriesAdded} {Resources.Strings.AppResources.NewCategories}";
                var pop2 = Toast.Make(text_cat, ToastDuration.Long, 14);
                await pop2.Show();
            }
            else
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                string text = $"{Resources.Strings.AppResources.SuccessAdded1} {transactionsAdded} {Resources.Strings.AppResources.SuccessAdded2}";
                var pop = Toast.Make(text, ToastDuration.Long, 14);
                await pop.Show(cancellationTokenSource.Token);
            }

#endif




        }
    }
}
