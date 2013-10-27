using System;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.DL
{
    public class DbFactoryRepository
    {
        public DbFactoryRepository()
        {
            RealDbFactory = new Lazy<DbFactory>(() => new DbFactory("Jeegoordah"));
            TestDbFactory = new Lazy<DbFactory>(() => new DbFactory("JeegoordahTest"));
        }
        
        public Lazy<DbFactory> RealDbFactory { get; private set; }
        public Lazy<DbFactory> TestDbFactory { get; private set; }
    }
}