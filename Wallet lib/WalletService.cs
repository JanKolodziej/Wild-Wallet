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
            return await _context.Transactions.Include(t => t.Category).Select(t => new Transactions
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Title = t.Title,
                CategoryId = t.CategoryId,
                AccountId = t.AccountId,
                Category=t.Category,
            }).OrderBy(t => t.Date).ToListAsync();
        }
        public async Task<List<AutomatedTransaction>> Get_All_Automated_Transactions_Async()
        {
            return await _context.AutomatedTransactions.Include(t => t.Category).Select(t=> new AutomatedTransaction
            {
                Id = t.Id,
                AccountId= t.AccountId,
                Category=t.Category,
                CategoryId=t.CategoryId,
                NextTime = t.NextTime,
                Frequency = t.Frequency,
                Amount=t.Amount,
                Name = t.Name,

            }).ToListAsync();
        }
        public async Task<List<Categories>> Get_All_Categories_Async()
        {
            var data = await _context.Categories.Select(c => new Categories
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Icon = c.Icon,
                Color = c.Color,
            }).ToListAsync();
            return data;
        }

        public async Task<List<Categories>> Get_All_Income_Categories_Async()
        {
            return await _context.Categories.Where(c => c.Type == "Income").Select(c => new Categories
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Icon = c.Icon,
                Color = c.Color
            }).ToListAsync();
        }
        public async Task<List<Categories>> Get_All_Expense_Categories_Async()
        {
            return await _context.Categories.Where(c => c.Type == "Expense").Select(c => new Categories
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Icon = c.Icon,
                Color = c.Color
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
        /// Calculates balance throughout whole history
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
        public async Task Add_AutomatedTransaction(AutomatedTransaction automatedTransaction)
        {
             _context.AutomatedTransactions.Add(automatedTransaction);
            await _context.SaveChangesAsync();
            await ProcessAutomatedTransactions();
        }
        public async Task<List<Transactions>> Get_Transaction_month(DateTime date)
        {
            var data = await _context.Transactions.AsNoTracking().Include(t => t.Category).Where(t => t.Date.Year == date.Date.Year && t.Date.Month == date.Date.Month).
                OrderByDescending(t => t.Date).ToListAsync();
            return data;
        }

        public async Task Delete_Transaction(Transactions transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task Delete_Automated_Transaction(AutomatedTransaction automatedTransaction)
        {
            _context.AutomatedTransactions.Remove(automatedTransaction);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> Get_Balance_To_Date(DateTime date)
        {
            var data = await _context.Transactions.Where(t => (t.Date.Month < date.Date.Month
            && t.Date.Year <= date.Date.Year) || t.Date.Year < date.Date.Year).SumAsync(t => t.Amount);
            return data;

        }
        public async Task Update_Transaction(Transactions transaction)
        {
            var data = await _context.Transactions.Where(t => t.Id == transaction.Id).FirstOrDefaultAsync();
            if (data != null)
            {
                data.Title = transaction.Title;
                data.Amount = transaction.Amount;
                data.CategoryId = transaction.CategoryId;
                data.AccountId = transaction.AccountId;
                data.Date = transaction.Date;
            }
            await _context.SaveChangesAsync();

        }

        public async Task ProcessAutomatedTransactions()
        {
            var duePayments = await _context.AutomatedTransactions.Where(p=>p.NextTime.Date<=DateTime.Now.Date.Date).ToListAsync();
            if(duePayments.Any())
            {
                foreach(var item in duePayments)
                {
                    while (item.NextTime.Date <= DateTime.Now.Date)
                    {
                        Transactions transaction = new(item.NextTime, item.Amount, $"{item.Name} |Tranzakcja Automatyczna|" , item.CategoryId, item.AccountId);
                        Add_Transaction(transaction);
                        item.NextTime = CalculateNextPayment(item.NextTime, item.Frequency);
                    }
                    
                }
                await _context.SaveChangesAsync();
            }
        }

        private DateTime CalculateNextPayment(DateTime dateTime, FrequencyOnceA onceA)
        {
            switch(onceA)
            {
                case FrequencyOnceA.Day:
                    return dateTime.AddDays(1);
                case FrequencyOnceA.Week:
                    return dateTime.AddDays(7);
                case FrequencyOnceA.Month:
                    return dateTime.AddMonths(1);
                case FrequencyOnceA.Year:
                    return dateTime.AddYears(1);
            }
            throw new Exception();
        }

    }

}
