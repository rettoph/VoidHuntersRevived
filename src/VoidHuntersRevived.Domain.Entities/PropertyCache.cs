using Svelto.ECS;
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

        public abstract void Initialize(ref EntityInitializer initializer);
    }

    internal class PropertyCache<T> : PropertyCache
        where T : class, IEntityProperty
    {
        public readonly T Instance;
        public readonly EntityPropertyConfiguration<T> Configuration;

        public PropertyCache(T instance, EntityPropertyConfiguration<T> configuration, int id) : base(id)
        {
            Instance = instance;
            Configuration = configuration;
        }

        public override void Initialize(ref EntityInitializer initializer)
        {
            this.Configuration.Initialize(this, ref initializer);
        }
    }
}
