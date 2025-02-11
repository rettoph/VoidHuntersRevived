using System.Reflection;
using System.Runtime.InteropServices;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Common.Extensions.System.Reflection;
using Guppy.Core.Common.Interfaces;
using Guppy.Core.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    public class ComponentSystemInvokerContext<TSequenceGroup>(
        Type[] type,
        SequenceGroup<TSequenceGroup> sequenceGroup)
            where TSequenceGroup : unmanaged, Enum
    {
        public readonly Type[] Types = type;
        public readonly SequenceGroup<TSequenceGroup> SequenceGroup = sequenceGroup;

        public override bool Equals(object? obj)
        {
            return obj is ComponentSystemInvokerContext<TSequenceGroup> context &&
                   Enumerable.SequenceEqual(this.Types, context.Types) &&
                   this.SequenceGroup == context.SequenceGroup;
        }

        public override int GetHashCode()
        {
            int typesHash = this.Types.Aggregate(0, (ag, t) => ag + t.GetHashCode());
            return HashCode.Combine(typesHash, this.SequenceGroup);
        }
    }

    public abstract class ComponentSystemInvoker
    {
        public delegate void ComponentSystemInvokerDelegate(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity);
        public class ComponentSystemInvokerDelegateSequenceGroup<TSequenceGroup>(bool sequence) : DelegateSequenceGroup<TSequenceGroup, ComponentSystemInvokerDelegate>(typeof(ComponentSystemInvokerDelegate), sequence)
            where TSequenceGroup : unmanaged, Enum
        {
            public void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
            {
                this.Sequenced?.Invoke(sourceEventId, entityTemplate, in entity);
            }
        }

        public abstract void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity);

        public static IEnumerable<ComponentSystemInvoker> Create<TSequenceGroup>(
            Type componentSystemInvokerType,
            Type systemType,
            IEnumerable<Type> componentTypes,
            IEnumerable<IScopedSystem> systems,
            EntitiesDB entitiesDB,
            Func<Type, MethodInfo> method
        )
            where TSequenceGroup : unmanaged, Enum
        {
            Dictionary<ComponentSystemInvokerContext<TSequenceGroup>, List<IScopedSystem>> validSystemsDictionary = [];

            foreach (IScopedSystem system in systems)
            { // Iterate through all systems...
                foreach (Type onComponentSystemType in system.GetType().GetConstructedGenericTypes(systemType))
                { // Select systems that specificaly implement the given system interface...
                    if (componentTypes.Intersect(onComponentSystemType.GenericTypeArguments).Count() == onComponentSystemType.GenericTypeArguments.Length)
                    { // Only look at system types that utilize the given list of components...
                        if (method(onComponentSystemType).TryGetSequenceGroup(system, true, out SequenceGroup<TSequenceGroup> sequenceGroup) == true)
                        { // Only look at systems with a defined sequence group
                            ComponentSystemInvokerContext<TSequenceGroup> context = new(onComponentSystemType.GenericTypeArguments, sequenceGroup);
                            ref List<IScopedSystem>? validSystemList = ref CollectionsMarshal.GetValueRefOrAddDefault(validSystemsDictionary, context, out _);
                            validSystemList ??= [];

                            validSystemList.Add(system);
                        }
                    }
                }
            }

            List<ComponentSystemInvoker> invokers = [];
            foreach (var (context, validSystemList) in validSystemsDictionary)
            {
                Type invokerType = componentSystemInvokerType.MakeGenericType(context.Types);
                ComponentSystemInvoker invoker = (ComponentSystemInvoker)Activator.CreateInstance(invokerType, context.SequenceGroup, validSystemList, entitiesDB)!;
                invokers.Add(invoker);
            }

            return invokers;
        }
    }

    public class OnSpawnSystemInvoker<TComponent>(SequenceGroup<OnSpawnSequenceGroupEnum> sequenceGroup, IEnumerable<IScopedSystem> systems, EntitiesDB entitiesDB) : ComponentSystemInvoker, IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnSpawnSystem<TComponent>[] _systems = systems.OfType<IOnSpawnSystem<TComponent>>().ToArray();

        SequenceGroup<OnSpawnSequenceGroupEnum> IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            ref TComponent component = ref this._entitiesDB.QueryEntityByIndex<TComponent>(entity.Index, entity.Group);
            Entity<TComponent> entityC = new(in entity, ref component);

            foreach (IOnSpawnSystem<TComponent> system in this._systems)
            {
                system.OnSpawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }

    public class OnSpawnSystemInvoker<TComponent1, TComponent2>(SequenceGroup<OnSpawnSequenceGroupEnum> sequenceGroup, IEnumerable<IScopedSystem> systems, EntitiesDB entitiesDB) : ComponentSystemInvoker, IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>
        where TComponent1 : unmanaged, IEntityComponent
        where TComponent2 : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnSpawnSystem<TComponent1, TComponent2>[] _systems = systems.OfType<IOnSpawnSystem<TComponent1, TComponent2>>().ToArray();

        SequenceGroup<OnSpawnSequenceGroupEnum> IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            var (component1s, component2s, _) = this._entitiesDB.QueryEntities<TComponent1, TComponent2>(entity.Group);
            Entity<TComponent1, TComponent2> entityC = new(in entity, ref component1s[entity.Index], ref component2s[entity.Index]);

            foreach (IOnSpawnSystem<TComponent1, TComponent2> system in this._systems)
            {
                system.OnSpawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }

    public class OnDespawnSystemInvoker<TComponent>(SequenceGroup<OnDespawnSequenceGroupEnum> sequenceGroup, IEnumerable<IScopedSystem> systems, EntitiesDB entitiesDB) : ComponentSystemInvoker, IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnDespawnSystem<TComponent>[] _systems = systems.OfType<IOnDespawnSystem<TComponent>>().ToArray();

        SequenceGroup<OnDespawnSequenceGroupEnum> IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            ref TComponent component = ref this._entitiesDB.QueryEntityByIndex<TComponent>(entity.Index, entity.Group);
            Entity<TComponent> entityC = new(in entity, ref component);

            foreach (IOnDespawnSystem<TComponent> system in this._systems)
            {
                system.OnDespawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }

    public class OnDespawnSystemInvoker<TComponent1, TComponent2>(SequenceGroup<OnDespawnSequenceGroupEnum> sequenceGroup, IEnumerable<IScopedSystem> systems, EntitiesDB entitiesDB) : ComponentSystemInvoker, IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>
        where TComponent1 : unmanaged, IEntityComponent
        where TComponent2 : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnDespawnSystem<TComponent1, TComponent2>[] _systems = systems.OfType<IOnDespawnSystem<TComponent1, TComponent2>>().ToArray();

        SequenceGroup<OnDespawnSequenceGroupEnum> IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            var (component1s, component2s, _) = this._entitiesDB.QueryEntities<TComponent1, TComponent2>(entity.Group);
            Entity<TComponent1, TComponent2> entityC = new(in entity, ref component1s[entity.Index], ref component2s[entity.Index]);

            foreach (IOnDespawnSystem<TComponent1, TComponent2> system in this._systems)
            {
                system.OnDespawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }
}