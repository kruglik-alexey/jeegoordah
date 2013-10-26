using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.Models;
using NHibernate;
using NHibernate.Linq;

namespace Jeegoordah.Web.Controllers
{
    public class EventsController : DbController
    {
        [HttpGet]
        public ActionResult List()
        {
            using (var db = DbFactory.OpenSession())
            {
                return Json(db.Query<Event>().Fetch(x => x.Bros).ToList().Select(e => new EventRest(e)), JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            using (var db = DbFactory.OpenSession())
            {
                return Json(new EventRest(db.Query<Event>().Fetch(x => x.Bros).First(e => e.Id == id)), JsonRequestBehavior.AllowGet);
            }
        }

//        [HttpGet]
//        public ActionResult GetTransactions(int id)
//        {
//            using (var db = DbFactory.OpenSession())
//            {
//                return Json(db.Transactions.Include("Targets").Include("Source").Include("Currency").Include("Event")
//                    .Where(t => t.Event.Id == id).OrderBy(t => t.Date).ThenBy(t => t.Id).ToList()
//                    .Select(t => new TransactionRest(t)), JsonRequestBehavior.AllowGet);
//            }
//        }

        [HttpPost]
        public ActionResult Create(EventRest @event)
        {            
            using (var db = DbFactory.OpenSession())
            {
                JsonResult nameAssertResult;
                if (!AssertNameUnique(@event, db, out nameAssertResult))
                {
                    return nameAssertResult;
                }   

                var dlEvent = new Event {CreatedAt = DateTime.UtcNow};
                @event.ToDataObject(dlEvent);
                db.Save(dlEvent);                
                return Json(new EventRest(dlEvent));
            }            
        }

        [HttpPost]
        public ActionResult Update(EventRest @event)
        {
            if (!@event.Id.HasValue)
            {
                Response.StatusCode = 400;
                return Json(new { Field = "Id", Message = "Missing Id" });
            }

            // TODO what if id invalid?
            using (var db = DbFactory.OpenSession())
            {                
                JsonResult nameAssertResult;
                if (!AssertNameUnique(@event, db, out nameAssertResult))
                {
                    return nameAssertResult;
                }

                var dlEvent = new Event
                {
                    CreatedAt = db.Query<Event>().Where(e => e.Id == @event.Id).Select(e => e.CreatedAt).First()
                };
                @event.ToDataObject(dlEvent);                
                db.Update(dlEvent);
                return Json(new EventRest(dlEvent));
            }            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO what if id invalid?
            using (var db = DbFactory.OpenSession())
            {
                db.Delete(db.Load<Event>(id));
            }
            // TODO respond just with header if OK
            return Json(new { });
        }

        private bool AssertNameUnique(EventRest @event, ISession db, out JsonResult result)
        {
            bool conflict;
            result = null;
            if (@event.Id.HasValue)
            {
                conflict = db.Query<Event>().Any(e => e.Id != @event.Id.Value && e.Name.Equals(@event.Name, StringComparison.CurrentCultureIgnoreCase));
            }
            else
            {
                conflict = db.Query<Event>().Any(e => e.Name.Equals(@event.Name, StringComparison.CurrentCultureIgnoreCase));
            }            
            if (!conflict) return true;

            result = Json(new {Field = "Name", Message = "Event with name {0} already exists.".F(@event.Name)});
            Response.StatusCode = 400;
            return false;
        }
    }
}