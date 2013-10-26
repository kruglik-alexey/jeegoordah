using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Jeegoordah.Web.Controllers
{
#if DEBUG
    public class TestController : DbController
    {
        [HttpGet]
        public void ClearDatabase()
        {
            using (var db = DbFactory.OpenSession())
            {
                if (!db.Connection.Database.Contains("test"))
                {
                    throw new InvalidOperationException("Can clear only test database");
                }
                db.CreateSQLQuery("DELETE FROM EVENTS").ExecuteUpdate();
            }
        }
    }
#endif
}