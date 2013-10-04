using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeegoordah.Core.DL
{
    public class JeegoordahDb : DbContext
    {
        static JeegoordahDb()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<JeegoordahDb>());
        }

        public JeegoordahDb(string connectionStringName) : base(connectionStringName) { }

        public DbSet<Event> Events { get; set; }
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
