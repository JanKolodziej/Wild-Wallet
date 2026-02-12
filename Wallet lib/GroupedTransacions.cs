using System.Collections.ObjectModel;

namespace Wallet_lib
{
    public class GroupedTransacions : ObservableCollection<Transactions>
    {
        public DateTime GroupDate { get; set; }
        public GroupedTransacions(DateTime date, IEnumerable<Transactions> transactions) : base(transactions)
        {
            GroupDate = date;
        }

    }
}
