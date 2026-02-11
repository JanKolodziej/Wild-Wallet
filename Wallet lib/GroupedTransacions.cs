
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_lib
{
    public class GroupedTransacions:ObservableCollection<Transactions>
    {
        public DateTime GroupDate {get;set;}
        public GroupedTransacions(DateTime date, IEnumerable<Transactions> transactions):base(transactions)
        {
            GroupDate = date;
        }
    }
}
