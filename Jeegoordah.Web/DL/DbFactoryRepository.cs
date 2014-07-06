using System;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.DL
{
    public class DbFactoryRepository
    {
        public DbFactoryRepository()
        {
            RealDbFactory = new Lazy<DbFactory>(() => new DbFactory("Jeegoordah"));
#if DEBUG
            TestDbFactory = new Lazy<DbFactory>(() => new DbFactory("JeegoordahTest"));
#endif
        }
        
        public Lazy<DbFactory> RealDbFactory { get; private set; }
#if DEBUG
        public Lazy<DbFactory> TestDbFactory { get; private set; }
#endif
    }
}