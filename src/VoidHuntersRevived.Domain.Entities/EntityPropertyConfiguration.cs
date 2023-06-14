using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities
{
    internal abstract class EntityPropertyConfiguration : IEntityPropertyConfiguration
    {
        internal abstract IEnumerable<IComponentBuilder> builders { get; }

        public abstract Type Type { get; }
    }

    internal sealed class EntityPropertyConfiguration<T> : EntityPropertyConfiguration, IEntityPropertyConfiguration<T>
        where T : class, IEntityProperty
    {
        private List<IComponentBuilder> _builders;
        internal override IEnumerable<IComponentBuilder> builders => _builders;

        public override Type Type => typeof(T);

        private HashSet<Type> _components;
        private InitializeComponentDelegate<T> _initializer;

        public EntityPropertyConfiguration()
        {
            _builders = new List<IComponentBuilder>();
            _components = new HashSet<Type>();
            _initializer = null!;

            this.HasComponent<Property<T>>();
            this.HasInitializer((T property, int id, ref EntityInitializer initializer) =>
            {
                initializer.Get<Property<T>>().Id = id;
            });
        }

        public void HasComponent<TComponent>() 
            where TComponent : unmanaged, IEntityComponent
        {
            _components.Add(typeof(TComponent));
            _builders.Add(new ComponentBuilder<TComponent>());
        }

        public void HasInitializer(InitializeComponentDelegate<T> initializer)
        {
            _initializer += initializer;
        }

        internal void Initialize(PropertyCache<T> cache, ref EntityInitializer initializer)
        {
            _initializer(cache.Instance, cache.Id, ref initializer);
        }
    }
}
