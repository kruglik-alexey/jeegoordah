using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Jeegoordah.Web.DL;

namespace Jeegoordah.Web.Controllers
{
#if DEBUG
    public class TestController : DbController
    {
        public TestController(ContextDependentDbFactory dbFactory) : base(dbFactory)
        {
        }

        [HttpGet]
        public void ClearDatabase()
        {
            using (var db = DbFactory.Open())
            {
                if (!db.Session.Connection.ConnectionString.Contains("test"))
                {
                    throw new InvalidOperationException("Can clear only test database");
                }
				Logger.I("Clear database");
                db.Session.CreateSQLQuery("DELETE FROM BROEVENTS").ExecuteUpdate();
                db.Session.CreateSQLQuery("DELETE FROM EVENT").ExecuteUpdate();
                db.Commit();
            }
        }
    }
#endif
}