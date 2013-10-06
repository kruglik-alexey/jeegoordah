using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Core.DL
{
    public class JeegoordahDb : DbContext
    {
        static JeegoordahDb()
        {
            Database.SetInitializer(new JeegoordahDbInitializer<JeegoordahRealDb>());
            Database.SetInitializer(new JeegoordahDbInitializer<JeegoordahTestDb>());
        }

        public JeegoordahDb(string connectionStringName) : base(connectionStringName) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Bro> Bros { get; set; }
    }

    public class JeegoordahRealDb : JeegoordahDb
    {
        public JeegoordahRealDb() : base("Jeegoordah") { }
    }

#if DEBUG
    public class JeegoordahTestDb : JeegoordahDb
    {
        public JeegoordahTestDb() : base("JeegoordahTest") { }
    }
#endif
}
