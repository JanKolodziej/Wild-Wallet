using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet_lib;

namespace Wallet_Main
{
    public class AddTransactionViewModel:ObservableObject
    {
        private readonly Wallet_lib.WalletService _service;
        public ObservableCollection<Wallet_lib.Transactions> Transactions_list { get; set; } = new();
        public AddTransactionViewModel(Wallet_lib.WalletService service)
        {
            _service = service;
        }

       
    }
}
