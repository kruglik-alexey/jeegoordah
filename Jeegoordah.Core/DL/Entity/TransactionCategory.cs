using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeegoordah.Core.DL.Entity
{
    public class TransactionCategory
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
