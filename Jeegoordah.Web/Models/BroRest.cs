using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class BroRest
    {
        public BroRest(Bro bro)
        {
            Id = bro.Id;
            Name = bro.Name;
	        IsHidden = bro.IsHidden;
        }

        public int Id { get; set; }
        public string Name { get; set; }
		public bool IsHidden { get; set; }
    }
}