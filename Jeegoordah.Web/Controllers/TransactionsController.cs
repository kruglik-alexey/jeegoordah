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
                db.Bros.Attach(dlTransaction.Source);
                dlTransaction.Targets.Where(t => t.Id != dlTransaction.Source.Id).ForEach(b => db.Bros.Attach(b));
                if (dlTransaction.Event != null)
                {
                    db.Events.Attach(dlTransaction.Event);
                }
                db.Currencies.Attach(dlTransaction.Currency);
                db.Transactions.Add(dlTransaction);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e )
                {
                    
                    throw;
                }           
                return Json(new TransactionRest(dlTransaction));
            }        
        }
    }
}