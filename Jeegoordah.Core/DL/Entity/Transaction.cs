using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeegoordah.Core.DL.Entity
{
    public class Transaction
    {
        [Key] public int Id { get; set; }

        [Required] public DateTime Date { get; set; }
        [Required] public DateTime CreatedAt { get; set; }        
        [Required] public Currency Currency { get; set; }
        [Required] public decimal Amount { get; set; }
        [Required] public Bro Source { get; set; }
        public List<Bro> Targets { get; set; }
        public Event Event { get; set; }
        public TransactionCategory Category { get; set; }
        public string Comment { get; set; }
    }
}
