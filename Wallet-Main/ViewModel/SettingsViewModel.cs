using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_Main
{
    public partial class SettingsViewModel
    {
        private readonly Wallet_lib.WalletService _service;
        public SettingsViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
        }
        [RelayCommand]
        public async Task Export_Transactions()
        {
            var transactions = await _service.Get_All_Transactions_Async();
            var to_export =new StringBuilder();
            to_export.AppendLine("Id;Date;Amount;Title;CategoryId;AccountId");
            foreach (var transaction in transactions) 
            {
                string title = string.Empty;
                if (transaction.Title != null)
                {
                     title = transaction.Title.Replace(';', ' ');
                }

               
                to_export.AppendLine($"{transaction.Id};{transaction.Date.ToString("yyyy-MM-dd")};{transaction.Amount};" +
                    $"{title};{transaction.CategoryId};{transaction.AccountId}");
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
    }
}
