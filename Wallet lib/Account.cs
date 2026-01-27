using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_lib
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set;  }
        public List<Transactions> Transactions { get; set; } = new List<Transactions>();

       public Account(string name, decimal balance = 0)
        {
            Name = name;
        }
        public void AddTransaction(Transactions transaction)
        {
            Transactions.Add(transaction);
        }
        public void RemoveTransaction(Transactions transaction)
        {
            Transactions.Remove(transaction);
        }



    }
}
