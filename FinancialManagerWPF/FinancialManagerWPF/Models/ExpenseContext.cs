using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagerWPF.Models
{
    public class ExpenseContext : DbContext
    {
        public ExpenseContext(): base("LoacalDbConnection")
        {

        }
        public void LoadAll()
        {
            expenses.Load();
            categories.Load();
            currencies.Load();
        }
        public DbSet<Expense> expenses { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Currency> currencies { get; set; }
    }
}
