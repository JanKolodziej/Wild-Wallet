using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet_lib
{
    public class MonthlyStat 
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Expenses { get; set; }
        public decimal Income {  get; set; }
        public decimal Balance => Income +Expenses;
        public MonthlyStat(DateTime date)
        {
            Month = date.Month;
            Year = date.Year;
        }
    
    }
}
