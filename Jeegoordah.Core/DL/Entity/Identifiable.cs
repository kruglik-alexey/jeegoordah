using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeegoordah.Core.DL.Entity
{
    public abstract class Identifiable : IEquatable<Identifiable>
    {
        public virtual int Id { get; set; }

        public virtual bool Equals(Identifiable other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identifiable);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
