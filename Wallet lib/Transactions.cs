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

        public Transactions()
        {

        }
        public Transactions( DateTime date, decimal amount, string? title, int? categoryId, int? accountId)
        {
            Date = date;
            Amount = amount;
            Title = title;
            CategoryId = categoryId;
            AccountId = accountId;
        }
    }
}
