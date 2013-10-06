using System.Data.Entity;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Core.DL
{
    public class JeegoordahDbInitializer<T> : DropCreateDatabaseIfModelChanges<T> where T : JeegoordahDb
    {
        protected override void Seed(T context)
        {
            context.Bros.Add(new Bro {Name = "Шылдон"});
            context.Bros.Add(new Bro {Name = "Мартен"});
            context.Bros.Add(new Bro {Name = "Алёна"});
            context.Bros.Add(new Bro {Name = "БлекД"});
            context.Bros.Add(new Bro {Name = "Моер"});
            context.Bros.Add(new Bro {Name = "Копыч"});
            context.Bros.Add(new Bro {Name = "Андрей"});
            context.Bros.Add(new Bro {Name = "Даша"});
            context.Bros.Add(new Bro {Name = "Сильвер"});
            context.Bros.Add(new Bro {Name = "Винни"});
            context.Bros.Add(new Bro {Name = "Лена"});
        }
    }
}