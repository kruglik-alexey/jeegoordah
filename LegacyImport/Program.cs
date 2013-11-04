using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;

namespace LegacyImport
{
    class Program
    {
        private static ISessionFactory sessionFactory;

        // legacy => 2.0
        private static Dictionary<int, int> broMap = new Dictionary<int, int>
        {
            {1, 1},
            {2, 2},
            {3, 3},
            {4, 4},
            {5, 8},
            {21, 7},
            {22, 5},
            {23, 9},
            {24, 6},
            {26, 10},
            {27, 11},
        };

        private static Dictionary<int, int> eventMap = new Dictionary<int, int>();

        private static IEnumerable<string[]> events;        
        private static IEnumerable<string[]> eventsPersons;
        private static IEnumerable<string[]> transactions;
        private static IEnumerable<string[]> directs;

        static void Main(string[] args)
        {
            CreateDatabase();
            LoadLegacyData();
            ImportData();
        }

        private static void ImportData()
        {
            using (var session = sessionFactory.OpenSession())
            {
                ImportEvents(session);
                session.Flush();
            }
        }

        private static void ImportEvents(ISession session)
        {
            var id = session.CreateSQLQuery("select seq from sqlite_sequence where name=event");
            var q = session.CreateSQLQuery("insert into event (name, startdate, description) values(:name, :startdate, :description)");
            q.SetParameter("startdate", "01-01-2013");
            q.SetParameter("description", "");
            foreach (string[] e in events)
            {
                q.SetParameter("name", e[1]);
                q.ExecuteUpdate();
                eventMap[int.Parse(e[0])] = id.List().As<int>();
            }
        }

        private static void LoadLegacyData()
        {
            events = LoadFile("Events");
            eventsPersons = LoadFile("EventsPersons");
            transactions = LoadFile("Transactions");
            directs = LoadFile("Directs");
        }

        private static IEnumerable<string[]> LoadFile(string file)
        {
            return File.ReadLines(@"..\..\Data\" + file + ".csv", Encoding.GetEncoding(1251)).Skip(1).Select(l => l.Split(';'));            
        }

        private static void CreateDatabase()
        {
            var sqliteConfig = SQLiteConfiguration.Standard.UsingFile("db.sqlite");
            var db = Fluently.Configure().Database(sqliteConfig);
            db.Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
            sessionFactory = db.BuildSessionFactory();
        }
    }
}
