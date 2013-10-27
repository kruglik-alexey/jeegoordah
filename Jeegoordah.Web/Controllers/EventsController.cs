﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.DL;
using Jeegoordah.Web.Models;
using NHibernate;
using NHibernate.Linq;

namespace Jeegoordah.Web.Controllers
{
    public class EventsController : DbController
    {
        public EventsController(ContextDependentDbFactory dbFactory) : base(dbFactory)
        {
        }

        [HttpGet]
        public ActionResult List()
        {
            using (var db = DbFactory.Open())
            {
                return Json(db.Query<Event>().Fetch(x => x.Bros).ToList().Select(e => new EventRest(e)), JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            using (var db = DbFactory.Open())
            {
                return Json(new EventRest(db.Query<Event>().First(e => e.Id == id)), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTransactions(int id)
        {
            using (var db = DbFactory.Open())
            {
                return Json(db.Query<Transaction>().Fetch(t => t.Targets).Where(t => t.Event.Id == id).ToList().Select(t => new TransactionRest(t)), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Create(EventRest @event)
        {            
            using (var db = DbFactory.Open())
            {
                JsonResult nameAssertResult;
                if (!AssertNameUnique(@event, db, out nameAssertResult))
                {
                    return nameAssertResult;
                }

                var dlEvent = EventFromRest(@event, db);
                dlEvent.CreatedAt = DateTime.UtcNow;
                db.Session.Save(dlEvent);
                return Json(new EventRest(dlEvent));
            }            
        }

        [HttpPost]
        public ActionResult Update(EventRest @event)
        {
            if (!@event.Id.HasValue)
            {
                Response.StatusCode = 400;
                return Json(new {Field = "Id", Message = "Missing Id"});
            }

            using (var db = DbFactory.Open())
            {                
                JsonResult nameAssertResult;
                if (!AssertNameUnique(@event, db, out nameAssertResult))
                {
                    return nameAssertResult;
                }

                var dlEvent = EventFromRest(@event, db);
                dlEvent.CreatedAt = db.Query<Event>().Where(e => e.Id == @event.Id).Select(e => e.CreatedAt).First();                
                db.Session.Update(dlEvent);
                return Json(new EventRest(dlEvent));
            }            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = DbFactory.Open())
            {
                db.Session.Delete(db.Load<Event>(id));
            }
            // TODO respond just with header if OK
            return Json(new { });
        }

        private bool AssertNameUnique(EventRest @event, Db db, out JsonResult result)
        {
            result = null;
            bool conflict = @event.Id.HasValue
                ? db.Query<Event>().Any(e => e.Id != @event.Id.Value && e.Name == @event.Name)
                : db.Query<Event>().Any(e => e.Name == @event.Name);
            if (!conflict) return true;

            result = Json(new {Field = "Name", Message = "Event with name {0} already exists.".F(@event.Name)});
            Response.StatusCode = 400;
            return false;
        }

        private Event EventFromRest(EventRest source, Db db)
        {
            var target = new Event();
            if (source.Id.HasValue)
            {
                target.Id = source.Id.Value;
            }
            target.Name = source.Name;
            target.StartDate = JsonDate.Parse(source.StartDate);
            target.Description = source.Description ?? "";
            target.Bros.AddRange(source.Bros.Load<Bro>(db));
            return target;
        }
    }
}