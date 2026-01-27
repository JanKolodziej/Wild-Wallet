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

        public Transactions(DateTime date, decimal amount, int categoryid,int accountid, string title = "")
        {
            Date = date;
            Amount = amount;
            Title = title;
            CategoryId = categoryid;
            AccountId = accountid; 

        }
    }
}
