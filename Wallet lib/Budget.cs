using System.ComponentModel.DataAnnotations.Schema;

namespace Wallet_lib
{
    public class Budget
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Categories Category { get; set; }
        public decimal Goal { get; set; }
        public Budget()
        {
        }
        public Budget(int categoryId, decimal goal, Categories categories)
        {
            CategoryId = categoryId;
            Goal = goal;
            Category = categories;
        }

    }
}
