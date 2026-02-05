using System.ComponentModel;

namespace Wallet_lib
{
    
    public class Transactions
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Title { get; set; } 
        public int? CategoryId { get; set; }
        public int? AccountId { get; set; }


    }
}
