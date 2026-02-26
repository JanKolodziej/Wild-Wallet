using System.Collections.ObjectModel;

namespace Wallet_lib
{
    public class GroupedTransactions : ObservableCollection<Transactions>
    {
        public DateTime GroupDate { get; set; }
        public GroupedTransactions(DateTime date, IEnumerable<Transactions> transactions) : base(transactions)
        {
            GroupDate = date;
        }

    }

    public class GroupedAutomatedTransactions: ObservableCollection<AutomatedTransaction>
    {
        public FrequencyOnceA Frequency { get; set; }
        public string FrequencyLabel => Labeler(Frequency);
        public string Labeler(FrequencyOnceA frequency)
        {
            return frequency switch
            {
                FrequencyOnceA.Day => "Codziennie",
                FrequencyOnceA.Week => "Co tydzień",
                FrequencyOnceA.Month => "Co miesiąc",
                FrequencyOnceA.Year => "Co rok",
                _ => "Inne"
            };
        }
        public GroupedAutomatedTransactions(FrequencyOnceA frequency, IEnumerable<AutomatedTransaction> transactions) : base(transactions)
        {
            Frequency = frequency;
        }
    }
}
