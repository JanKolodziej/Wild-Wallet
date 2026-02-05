using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
                }).ToListAsync();
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
                Id =a.Id,
                Name = a.Name,

            }).ToListAsync();
        }
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
            var data = await _context.Transactions.Where(t=> t.Date.Year ==date.Date.Year && t.Date.Month==date.Date.Month).ToListAsync();
            return data;
        }

        
    }

}
