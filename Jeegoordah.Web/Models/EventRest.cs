using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Jeegoordah.Core;
using Jeegoordah.Core.DL.Entity;
using NHibernate;

namespace Jeegoordah.Web.Models
{
    public class EventRest : IValidatableObject
    {
        public EventRest()
        {
            Bros = new List<int>();
        }

        public EventRest(Event source)
        {
            Id = source.Id;
            Name = source.Name;
            Description = source.Description;
            StartDate = JsonDate.ToString(source.StartDate);
            Bros = source.Bros.Select(b => b.Id).ToList();
        }

        public int? Id { get; set; }
        [Required] public string Name { get; set; }        
        [Required] public string StartDate { get; set; }
        public string Description { get; set; }
        public List<int> Bros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Bros.Distinct().Count() < 2)
                yield return new ValidationResult("Event should has at least two Bros", new[] {"Bros"});
        }
    }
}