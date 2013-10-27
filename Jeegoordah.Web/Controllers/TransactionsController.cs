using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.DL;
using Jeegoordah.Web.Models;
using Jeegoordah.Web.Models.Validation;

namespace Jeegoordah.Web.Controllers
{
    public class TransactionsController : DbController
    {
        public TransactionsController(ContextDependentDbFactory dbFactory) : base(dbFactory)
        {
        }

        [HttpPost]
        [ValidateModelState]
        public ActionResult Create(TransactionRest transaction)
        {
            using (var db = DbFactory.Open())
            {
                var dlTransaction = new Transaction {CreatedAt = DateTime.UtcNow};
                transaction.ToDataObject(dlTransaction);
                dlTransaction.Source = transaction.Source.Load<Bro>(db);
                dlTransaction.Targets.AddRange(transaction.Targets.Load<Bro>(db));
                if (transaction.Event.HasValue)
                {
                    dlTransaction.Event = transaction.Event.Value.Load<Event>(db);
                }
                dlTransaction.Currency = transaction.Currency.Load<Currency>(db);
                db.Session.Save(dlTransaction);
                return Json(new TransactionRest(dlTransaction));
            }        
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = DbFactory.Open())
            {
                db.Session.Delete(db.Load<Transaction>(id));
            }
            return Json(new { });
        }
    }
}