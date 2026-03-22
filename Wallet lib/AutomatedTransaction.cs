using System.ComponentModel.DataAnnotations.Schema;

namespace Wallet_lib
{
    public enum FrequencyOnceA
    {
        Day = 0,
        Week = 1,
        Month = 2,
        Year = 3,
    }
    public class AutomatedTransaction
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int? AccountId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Categories Category { get; set; }

        public FrequencyOnceA Frequency { get; set; }
        public DateTime NextTime { get; set; }

    }
}
