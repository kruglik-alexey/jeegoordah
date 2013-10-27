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
    public class EventRest
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
        [Required] public string Description { get; set; }
        [Required] public string StartDate { get; set; }
        public List<int> Bros { get; set; }

        public void ToDataObject(Event target)
        {
            if (Id.HasValue)
            {
                target.Id = Id.Value;
            }
            target.Name = Name;
            target.StartDate = JsonDate.Parse(StartDate);
            target.Description = Description ?? "";
            target.Bros.Clear();
        }
    }
}