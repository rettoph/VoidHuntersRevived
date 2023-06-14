using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities
{
    internal abstract class PropertyCache
    {
        public readonly int Id;

        protected PropertyCache(int id)
        {
            Id = id;
        }
    }

    internal class PropertyCache<T> : PropertyCache
        where T : IEntityProperty
    {
        public readonly T Instance;

        public PropertyCache(T instance, int id) : base(id)
        {
            Instance = instance;
        }
    }
}
