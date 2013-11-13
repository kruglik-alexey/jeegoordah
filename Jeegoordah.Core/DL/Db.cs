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
            this.transaction = hibernateSession.BeginTransaction();
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

        public void Dispose()
        {
            if (Session == null) return;
            var session = Session;
            Session = null;

            try
            {
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            session.Flush();
            session.Dispose();
        }
    }
}
