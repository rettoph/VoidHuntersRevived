using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Systems;
using VoidHuntersRevived.Domain.ECS.Services;

namespace VoidHuntersRevived.Domain.ECS.Abstractions
{
    internal sealed class ReactiveEngine<T> : IEngine, IReactOnAddEx<Component<T>>, IReactOnAddAndRemoveEx<Component<T>>
        where T : unmanaged
    {
        private readonly EntityService _entities;
        private readonly IReactiveSystem<T> _system;

        public ReactiveEngine(EntityService entities, IReactiveSystem<T> system)
        {
            _entities = entities;
            _system = system;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Component<T>> entities, ExclusiveGroupStruct groupID)
        {
            var (components, entityIds, _) = entities;

            for(uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId entityKey = _entities.GetEntityKey(entityIds[index], groupID);
                Ref<T> component = new Ref<T>(ref components[index].Instance);

                _system.OnAdded(in entityKey, in component);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Component<T>> entities, ExclusiveGroupStruct groupID)
        {
            var (components, entityIds, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId entityKey = _entities.GetEntityKey(entityIds[index], groupID);
                Ref<T> component = new Ref<T>(ref components[index].Instance);

                _system.OnRemoved(in entityKey, in component);
            }
        }
    }
}
