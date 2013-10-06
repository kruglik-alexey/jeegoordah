using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jeegoordah.Core.DL.Entity
{
    public class Event
    {
        public Event()
        {
            Bros = new List<Bro>();
        }

        [Key] 
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
         
        public List<Bro> Bros { get; set; }
    }
}
