using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.Models;

namespace Jeegoordah.Web.Controllers
{
    public class TransactionsController : DbController
    {
        [HttpPost]
        public ActionResult Create(TransactionRest transaction)
        {
            using (var db = DbFactory.CreateDb())
            {
                var dlTransaction = new Transaction {CreatedAt = DateTime.UtcNow};
                transaction.ToDataObject(dlTransaction);
                dlTransaction.Source = db.Bros.Find(transaction.Source);
                dlTransaction.Targets = transaction.Targets.Select(t => db.Bros.Find(t)).ToList();
                if (transaction.Event.HasValue)
                {
                    dlTransaction.Event = db.Events.Find(transaction.Event.Value);
                }
                dlTransaction.Currency = db.Currencies.Find(transaction.Currency);
                db.Transactions.Add(dlTransaction);
                db.SaveChanges();                       
                return Json(new TransactionRest(dlTransaction));
            }        
        }
    }
}