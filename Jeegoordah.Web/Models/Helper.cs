using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL;
using NHibernate;

namespace Jeegoordah.Web.Models
{
    public static class Helper
    {
        public static IEnumerable<T> Load<T>(this IEnumerable<int> ids, Db db)
        {
            return ids.Select(id => db.Load<T>(id));
        }

        public static T Load<T>(this int id, Db db)
        {
            return db.Load<T>(id);
        }

        public static T Load<T>(this int? id, Db db)
        {
            return db.Load<T>(id.Value);
        }
    }
}