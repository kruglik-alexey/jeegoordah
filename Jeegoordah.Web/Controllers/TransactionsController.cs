using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL;
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
                var dlTransaction = TransactionFromRest(transaction, db);
                db.Session.Save(dlTransaction);
	            var response = new TransactionRest(dlTransaction);
				Logger.I("Created transaction {0}", response.ToJson());
                return Json(response);
            }        
        }

        [HttpPost]
        [ValidateModelState]
        public ActionResult Update(TransactionRest transaction)
        {
            if (!transaction.Id.HasValue)
            {
				Logger.I("Attempt to update transaction without id");
                Response.StatusCode = 400;
                return Json(new { Field = "Id", Message = "Missing Id" });
            }

            using (var db = DbFactory.Open())
            {
                var dlTransaction = TransactionFromRest(transaction, db);
                db.Session.Update(dlTransaction);
				var response = new TransactionRest(dlTransaction);
				Logger.I("Updated transaction {0}", response.ToJson());
				return Json(response);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = DbFactory.Open())
            {
                db.Session.Delete(db.Load<Transaction>(id));
				Logger.I("Deleted transaction {0}", id);
            }
            return Json(new { });
        }

        [HttpGet]
        public ActionResult ListP2P()
        {
            using (var db = DbFactory.Open())
            {
                return Json(db.Query<Transaction>()
                            .Where(t => t.Event == null)
                            .OrderByDescending(t => t.Date)
                            .ToList()
                            .Select(t => new TransactionRest(t))
                            // TODO why we need ToList() here?
                            .ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        private Transaction TransactionFromRest(TransactionRest source, Db db)
        {
            var target = new Transaction();
            if (source.Id.HasValue)
            {
                target.Id = source.Id.Value;
            }
            target.Date = JsonDate.Parse(source.Date);
            target.Amount = source.Amount.Value;
            target.Comment = source.Comment ?? "";

            target.Source = source.Source.Load<Bro>(db);
            target.Targets.AddRange(source.Targets.Load<Bro>(db));
            if (source.Event.HasValue)
            {
                target.Event = source.Event.Value.Load<Event>(db);
            }
            target.Currency = source.Currency.Load<Currency>(db);

            return target;
        }
    }
}