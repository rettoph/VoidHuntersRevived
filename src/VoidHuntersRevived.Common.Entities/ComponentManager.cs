using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public abstract class ComponentManager
    {
        public readonly IComponentBuilder Builder;

        protected ComponentManager(IComponentBuilder builder)
        {
            this.Builder = builder;
        }

        public abstract void Clone(uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entities, ref EntityInitializer clone);
    }

    public sealed class ComponentManager<TComponent> : ComponentManager
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentManager() : base(new ComponentBuilder<TComponent>())
        {

        }

        public override void Clone(uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entities, ref EntityInitializer clone)
        {
            var (components, _) = entities.QueryEntities<TComponent>(groupId);
            clone.Init(components[sourceIndex]);
        }
    }
}
