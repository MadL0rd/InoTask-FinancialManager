using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FinancialManagerWPF.Models
{
    public class Expense
    {
        [Key]
        public int id { get; set; }
        public float value { get; set; }
        public DateTime date { get; set; }

        public Category category { get; set; }
        public Currency currency { get; set; }
    }
    public class Group
    {
        public int id { get; set; }

        [StringLength(30)]
        public string title { get; set; }
        [StringLength(9)]
        public string color { get; set; }
    }
    public class Category : Group { }

    public class Currency : Group
    {
        public int balance { get; set; }
    }

}
