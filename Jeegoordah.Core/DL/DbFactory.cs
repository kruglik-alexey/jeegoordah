using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Linq;
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
            db.ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true));
            sessionFactory = db.BuildSessionFactory();
            Seed();
        }

        private void Seed()
        {
            using (var db = Open())
            {
                if (!db.Query<Currency>().Any())
                    DbSeed.Seed(db.Session);
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
