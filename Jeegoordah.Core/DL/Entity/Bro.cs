using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jeegoordah.Core.DL.Entity
{
    public class Bro
    {
        public Bro()
        {
            Events = new List<Event>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<Event> Events { get; set; }
    }
}
