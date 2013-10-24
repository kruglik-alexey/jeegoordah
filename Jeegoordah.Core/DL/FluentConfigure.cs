using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Jeegoordah.Core.DL.Entity;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Jeegoordah.Core.DL
{
    public static class FluentConfigure
    {
        public static void Configure()
        {
            var config = Fluently.Configure().Database(SQLiteConfiguration.Standard.ConnectionString(
                c => c.FromConnectionStringWithKey("JeegoordahSQLite")))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
            config.ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true));
            var f = config.BuildSessionFactory();
            var s = f.OpenSession();
            s.Save(new Currency {Name = "USD"});
            s.Flush();
            s.Dispose();
        }
    }
}
