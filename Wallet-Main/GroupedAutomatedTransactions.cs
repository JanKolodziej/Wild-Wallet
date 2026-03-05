
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_Main
{
    public class GroupedAutomatedTransactions : ObservableCollection<Wallet_lib.AutomatedTransaction>
    {
        public Wallet_lib.FrequencyOnceA Frequency { get; set; }
        public string FrequencyLabel => Labeler(Frequency);
        public string Labeler(Wallet_lib.FrequencyOnceA frequency)
        {
            return frequency switch
            {
                Wallet_lib.FrequencyOnceA.Day => Resources.Strings.AppResources.Daily,
                Wallet_lib.FrequencyOnceA.Week => Resources.Strings.AppResources.Weekly,
                Wallet_lib.FrequencyOnceA.Month => Resources.Strings.AppResources.Monthly,
                Wallet_lib.FrequencyOnceA.Year => Resources.Strings.AppResources.Yearly,
            };
        }   
        public GroupedAutomatedTransactions(Wallet_lib.FrequencyOnceA frequency, IEnumerable<Wallet_lib.AutomatedTransaction> transactions) : base(transactions)
        {
            Frequency = frequency;
        }
    }
}
