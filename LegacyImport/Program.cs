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

        // 2.0 stuff
        private static Dictionary<int, List<int>> eventBroMap = new Dictionary<int, List<int>>();

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
                ImportEventBros(session);
                var id = ImportTransactions(session);
                ImportDirects(session, id);
                session.Flush();
            }
        }

        private static void ImportDirects(ISession session, int id)
        {
            var q = session.CreateSQLQuery(
                "insert into [transaction] (date, amount, comment, currency_id, source_id) values(:date, :amount, :comment, :currency, :source)");
            var qq = session.CreateSQLQuery("insert into transactiontargets (transaction_id, bro_id) values(:transaction, :bro)");
            q.SetParameter("currency", 1);
            q.SetParameter("date", "2013-01-01 00:00:00");
            foreach (string[] transaction in directs)
            {
                q.SetParameter("amount", transaction[3]);
                q.SetParameter("comment", transaction[4]);
                q.SetParameter("source", broMap[int.Parse(transaction[2])]);
                q.ExecuteUpdate();

                qq.SetParameter("transaction", id);
                qq.SetParameter("bro", broMap[int.Parse(transaction[1])]);
                qq.ExecuteUpdate();
                id++;
            }
        }

        private static int ImportTransactions(ISession session)
        {
            var q = session.CreateSQLQuery(
                "insert into [transaction] (date, amount, comment, currency_id, source_id, event_id) values(:date, :amount, :comment, :currency, :source, :event)");
            var qq = session.CreateSQLQuery("insert into transactiontargets (transaction_id, bro_id) values(:transaction, :bro)");
            q.SetParameter("currency", 1);
            q.SetParameter("date", "2013-01-01 00:00:00");
            int id = 1;
            foreach (string[] transaction in transactions)
            {
                var @event = eventMap[int.Parse(transaction[2])];
                q.SetParameter("amount", transaction[3]);
                q.SetParameter("comment", transaction[4]);
                q.SetParameter("source", broMap[int.Parse(transaction[1])]);
                q.SetParameter("event", @event);
                q.ExecuteUpdate();

                foreach (int bro in eventBroMap[@event])
                {
                    qq.SetParameter("transaction", id);
                    qq.SetParameter("bro", bro);
                    qq.ExecuteUpdate();
                }
                id++;
            }
            return id;
        }

        private static void ImportEventBros(ISession session)
        {
            var q = session.CreateSQLQuery("insert into broevents (bro_id, event_id) values(:broid, :eventid)");
            foreach (string[] eventsPerson in eventsPersons)
            {
                int bro = broMap[int.Parse(eventsPerson[2])];
                int @event = eventMap[int.Parse(eventsPerson[1])];
                q.SetParameter("broid", bro);
                q.SetParameter("eventid", @event);
                q.ExecuteUpdate();

                List<int> bros;
                if (!eventBroMap.TryGetValue(@event, out bros))
                {
                    bros = new List<int>();
                    eventBroMap[@event] = bros;
                }
                bros.Add(bro);
            }
        }

        private static void ImportEvents(ISession session)
        {
            var q = session.CreateSQLQuery("insert into event (name, startdate, description) values(:name, :startdate, :description)");
            q.SetParameter("startdate", "2013-01-01 00:00:00");
            q.SetParameter("description", "");
            int id = 1;
            foreach (string[] e in events)
            {
                q.SetParameter("name", e[1]);
                q.ExecuteUpdate();
                eventMap[int.Parse(e[0])] = id;
                id++;
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
