using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.Controllers
{
    public class GeneralController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {     
            return View();
        }

        [HttpGet]
        public ActionResult Events()
        {
            using (var db = new JeegoordahDb())
            {
                return Json(db.Events.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateEvent(Event @event)
        {
            using (var db = new JeegoordahDb())
            {
                if (db.Events.Any(e => e.Name.Equals(@event.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    // TODO or should it be still 200?
                    Response.StatusCode = 400;
                    return Json(new {Field = "Name", Message = "Event with name {0} already exists.".F(@event.Name)});
                }
                @event.CreatedAt = DateTime.UtcNow;
                db.Events.Add(@event);
                db.SaveChanges();
            }
            return Json(@event);
        }

        [HttpPost]
        public ActionResult UpdateEvent(Event @event)
        {
            // TODO what if id invalid?
            using (var db = new JeegoordahDb())
            {
                if (db.Events.Any(e => e.Name.Equals(@event.Name, StringComparison.CurrentCultureIgnoreCase) && e.Id != @event.Id))
                {
                    // TODO or should it be still 200?
                    Response.StatusCode = 400;
                    return Json(new {Field = "Name", Message = "Event with name {0} already exists.".F(@event.Name)});
                }
                @event.CreatedAt = db.Events.Where(e => e.Id == @event.Id).Select(e => e.CreatedAt).First();
                db.Events.Attach(@event);
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(@event);
        }

        [HttpPost]
        public ActionResult DeleteEvent(int id)
        {
            // TODO what if id invalid?
            using (var db = new JeegoordahDb())
            {
                var e = new Event {Id = id};
                db.Events.Attach(e);
                db.Events.Remove(e);
                db.SaveChanges();
            }
            // TODO respond just with header if OK
            return Json(new {});
        }
    }
}
