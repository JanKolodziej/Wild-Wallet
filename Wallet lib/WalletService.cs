using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_lib
{
    public class WalletService
    {
        private readonly WalletContext _context;
        public WalletService()
        {
            _context = new WalletContext();
        }


        public async Task<List<Transactions>> GetAllTransactionsAsync()
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
        
        public async Task<List<Categories>> GetAllCategoriesAsync()
        {
            return await _context.Categories.Select(c => new Categories
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync();
        }
        public async Task<List<Accounts>> GetAccountsAsync()
        {
            return await _context.Accounts.Select(a => new Accounts
            {
                Id =a.Id,
                Name = a.Name,

            }).ToListAsync();
        }

        
    }

}
