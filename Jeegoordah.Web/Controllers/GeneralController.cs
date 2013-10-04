using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.Controllers
{
    public class GeneralController : DbController
    {
        public class CacheBuster
        {
            private readonly string _cacheBuster;

            public CacheBuster()
            {
#if DEBUG
                _cacheBuster = (new Random()).Next().ToString();
#else
                _cacheBuster = "2.0.0";
#endif
            }

            public string RenderCssLink(string cssPath)
            {
                return @"<link href=""{0}?{1}"" type=""text/css"" rel=""stylesheet""/>".F(cssPath, _cacheBuster);
            }

            public string RenderRequireJsCacheBuster(string requireJsPath, string mainScriptPath)
            {
                return @"<script type=""text/javascript"">window.jgdhCacheBuster = '{0}'</script>
                     <script data-main=""{1}?{0}"" src=""{2}?{0}"" type=""text/javascript""></script>".F(_cacheBuster, mainScriptPath, requireJsPath);
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new CacheBuster());
        }

        [HttpGet]
        public ActionResult Events()
        {
            using (var db = DbFactory.CreateDb())
            {
                return Json(db.Events.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateEvent(Event @event)
        {
            using (var db = DbFactory.CreateDb())
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
            using (var db = DbFactory.CreateDb())
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
            using (var db = DbFactory.CreateDb())
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
