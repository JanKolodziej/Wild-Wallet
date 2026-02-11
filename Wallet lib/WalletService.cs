using Microsoft.EntityFrameworkCore;

namespace Wallet_lib
{
    public class WalletService
    {
        private readonly WalletContext _context;
        public WalletService(WalletContext context)
        {
            _context = context;
        }


        public async Task<List<Transactions>> Get_All_Transactions_Async()
        {
            return await _context.Transactions.Select(t => new Transactions
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Title = t.Title,
                CategoryId = t.CategoryId,
                AccountId = t.AccountId
            }).OrderBy(t => t.Date).ToListAsync();
        }

        public async Task<List<Categories>> Get_All_Categories_Async()
        {
            return await _context.Categories.Select(c => new Categories
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync();
        }
        public async Task<List<Accounts>> Get_Accounts_Async()
        {
            return await _context.Accounts.Select(a => new Accounts
            {
                Id = a.Id,
                Name = a.Name,

            }).ToListAsync();
        }
        /// <summary>
        /// Calculates balance throughout whole histoty
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> Get_Balance_Async()
        {
            return await _context.Transactions.SumAsync(t => t.Amount);
        }
        /// <summary>
        /// Calculates income in a given month
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> Get_Income_Async(DateTime date)
        {
            return await _context.Transactions.Where(t => t.Amount > 0 && t.Date.Month == date.Month).SumAsync(t => t.Amount);
        }
        /// <summary>
        /// Calculates expenses in a given month
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> Get_Expenses_Async(DateTime date)
        {
            return await _context.Transactions.Where(t => t.Amount < 0 && t.Date.Month == date.Month).SumAsync(t => t.Amount);
        }
        /// <summary>
        /// Adds a new transaction to the data base 
        /// </summary>
        /// <param name="transaction">The <see cref="Transactions"/> object to add. Cannot be <c>null</c>.</param>
        public async void Add_Transaction(Transactions transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
        public async void Add_Account(Accounts account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }
        public async void Add_Category(Categories categories)
        {
            _context.Categories.Add(categories);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Transactions>> Get_Transaction_month(DateTime date)
        {
            var data = await _context.Transactions.Where(t => t.Date.Year == date.Date.Year && t.Date.Month == date.Date.Month).
                OrderByDescending(t => t.Date).ToListAsync();
            return data;
        }

        public async Task Delete_Transaction(Transactions transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> Get_Balance_To_Date(DateTime date)
        {
            var data = await _context.Transactions.Where(t => (t.Date.Month < date.Date.Month
            && t.Date.Year <= date.Date.Year) || t.Date.Year < date.Date.Year).SumAsync(t => t.Amount);
            return data;

        }


    }

}
