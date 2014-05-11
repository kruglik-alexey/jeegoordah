using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Jeegoordah.Core.Logging;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Core.DL
{
    public class DbFactory : IDisposable
    {
        private readonly ISessionFactory sessionFactory;

        public DbFactory(string connectionStringName)
        {
            var sqliteConfig = SQLiteConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey(connectionStringName)).ShowSql();			
            var db = Fluently.Configure().Database(sqliteConfig);
            db.Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
            db.ExposeConfiguration(c =>
	            {
		            new SchemaUpdate(c).Execute(false, true);
					Logger.For(this).I("Use connection string '{0}'", c.GetProperty(NHibernate.Cfg.Environment.ConnectionString));
	            });
            sessionFactory = db.BuildSessionFactory();
            Seed();
        }

        private void Seed()
        {
            using (var db = Open())
            {
                if (!db.Query<Currency>().Any())
                    DbSeed.Seed(db.Session);
                db.Commit();
            }
        }

        public Db Open()
        {
            return new Db(sessionFactory.OpenSession());
        }

        public void Dispose()
        {
            sessionFactory.Dispose();
        }
    }
}
