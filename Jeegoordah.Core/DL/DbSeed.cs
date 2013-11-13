using System.Data.Entity;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Core.Logging;
using NHibernate;

namespace Jeegoordah.Core.DL
{
    public static class DbSeed
    {
        public static void Seed(ISession session)
        {
			Logger.For(typeof(DbSeed)).I("Seed db");

            session.Save(new Currency {Name = "BYR", Accuracy = 3});
            session.Save(new Currency {Name = "USD", Accuracy = 0});

            session.Save(new Bro {Name = "Шылдон"});
            session.Save(new Bro {Name = "Мартен"});
            session.Save(new Bro {Name = "Алёна"});
            session.Save(new Bro {Name = "БлекД"});
            session.Save(new Bro {Name = "Моер"});
            session.Save(new Bro {Name = "Копыч"});
            session.Save(new Bro {Name = "Андрей"});
            session.Save(new Bro {Name = "Даша"});
            session.Save(new Bro {Name = "Сильвер"});
            session.Save(new Bro {Name = "Винни"});
            session.Save(new Bro {Name = "Лена"});
        }
    }
}