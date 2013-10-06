using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.Models;

namespace Jeegoordah.Web.Controllers
{
    public class EventsController : DbController
    {
        [HttpGet]
        public ActionResult List()
        {
            using (var db = DbFactory.CreateDb())
            {
                return Json(db.Events.Include("Bros").ToList().Select(e => new EventRest(e)), JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpPost]
        public ActionResult Create(EventRest @event)
        {            
            using (var db = DbFactory.CreateDb())
            {
                if (db.Events.Any(e => e.Name.Equals(@event.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    // TODO or should it be still 200?
                    Response.StatusCode = 400;
                    return Json(new {Field = "Name", Message = "Event with name {0} already exists.".F(@event.Name)});
                }

                var dlEvent = new Event {CreatedAt = DateTime.UtcNow};
                @event.ToDataObject(dlEvent);
                dlEvent.Bros.ForEach(b => db.Bros.Attach(b));
                db.Events.Add(dlEvent);                               
                db.SaveChanges();                            
                return Json(new EventRest(dlEvent));
            }            
        }

        [HttpPost]
        public ActionResult Update(EventRest @event)
        {
            // TODO what if id invalid?
            using (var db = DbFactory.CreateDb())
            {
                if (db.Events.Any(e => e.Name.Equals(@event.Name, StringComparison.CurrentCultureIgnoreCase) && e.Id != @event.Id))
                {
                    // TODO or should it be still 200?
                    Response.StatusCode = 400;
                    return Json(new {Field = "Name", Message = "Event with name {0} already exists.".F(@event.Name)});
                }

                List<int> oldBros = db.Bros.Where(b => b.Events.Any(e => e.Id == @event.Id)).Select(b => b.Id).ToList();
                var dlEvent = new Event
                {
                    Id = @event.Id,
                    CreatedAt = db.Events.Where(e => e.Id == @event.Id).Select(e => e.CreatedAt).First(),
                    Bros = oldBros.Select(b => new Bro {Id = b}).ToList()
                };
                dlEvent.Bros.ForEach(b => db.Bros.Attach(b));
                db.Events.Attach(dlEvent);                
                @event.ToDataObject(dlEvent);
                dlEvent.Bros.Where(b => !oldBros.Contains(b.Id)).ToList().ForEach(b => db.Bros.Attach(b));
                db.SaveChanges();
                return Json(new EventRest(dlEvent));
            }            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO what if id invalid?
            using (var db = DbFactory.CreateDb())
            {
                var e = new Event { Id = id };
                db.Events.Attach(e);
                db.Events.Remove(e);
                db.SaveChanges();
            }
            // TODO respond just with header if OK
            return Json(new { });
        }
    }
}