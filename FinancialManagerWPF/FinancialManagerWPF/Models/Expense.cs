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
    public class BaseData
    {
        [Key]
        public int id { get; set; }
        [StringLength(30)]
        public string title { get; set; }
    }
    public class Expense : BaseData
    {
        public double value { get; set; }
        public DateTime date { get; set; }
        public Category category { get; set; }
        public Currency currency { get; set; }
    }


    public class Group:BaseData
    {   
        [StringLength(9)]
        public string color { get; set; }
    }
    public class Category : Group { }

    public class Currency : Group
    {
        public double balance { get; set; }
    }

}
