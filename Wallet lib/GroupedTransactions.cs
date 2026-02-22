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
}
