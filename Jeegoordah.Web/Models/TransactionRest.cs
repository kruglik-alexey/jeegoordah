﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class TransactionRest : IValidatableObject
    {
        public TransactionRest()
        {
            Targets = new List<int>();
        }

        public TransactionRest(Transaction transaction)
        {
            Id = transaction.Id;
            Date = JsonDate.ToString(transaction.Date);
            Amount = transaction.Amount;
            Rate = transaction.Rate.ToString(CultureInfo.InvariantCulture);
            Currency = transaction.Currency.Id;
            Source = transaction.Source.Id;
            Targets = transaction.Targets.Select(t => t.Id).ToList();
            Event = transaction.Event != null ? transaction.Event.Id : (int?)null; 
            Comment = transaction.Comment;
        }
        
        public int? Id { get; set; }
        [Required] public string Date { get; set; }
        [Required] public decimal Amount { get; set; }
        [Required] public string Rate { get; set; }
        [Required] public int Currency { get; set; }
        [Required] public int Source { get; set; }
        public List<int> Targets { get; set; }
        public int? Event { get; set; }
        public string Comment { get; set; }        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Targets.Any())
                yield return new ValidationResult("Empty Targets", new[] {"Targets"});
        }
    }
}