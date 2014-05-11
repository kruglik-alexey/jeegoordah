using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;

namespace Jeegoordah.Core.DL
{
    public class Db : IDisposable
    {
        private readonly ITransaction transaction;

        public Db(ISession hibernateSession)
        {
            Session = hibernateSession;
            transaction = hibernateSession.BeginTransaction();
        }

        public ISession Session { get; private set; }

        public IQueryable<T> Query<T>()
        {
            return Session.Query<T>();
        }

        public T Load<T>(int id)
        {
            return Session.Load<T>(id);
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Dispose()
        {                 
            transaction.Dispose();            
            Session.Dispose();
        }
    }
}
