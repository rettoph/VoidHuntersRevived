using System.Reflection;
using System.Runtime.InteropServices;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Common.Extensions.System.Reflection;
using Guppy.Core.Common.Interfaces;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    public class ComponentEngineInvokerContext<TSequenceGroup>(
        Type[] type,
        SequenceGroup<TSequenceGroup> sequenceGroup)
            where TSequenceGroup : unmanaged, Enum
    {
        public readonly Type[] Types = type;
        public readonly SequenceGroup<TSequenceGroup> SequenceGroup = sequenceGroup;

        public override bool Equals(object? obj)
        {
            return obj is ComponentEngineInvokerContext<TSequenceGroup> context &&
                   Enumerable.SequenceEqual(this.Types, context.Types) &&
                   this.SequenceGroup == context.SequenceGroup;
        }

        public override int GetHashCode()
        {
            int typesHash = this.Types.Aggregate(0, (ag, t) => ag + t.GetHashCode());
            return HashCode.Combine(typesHash, this.SequenceGroup);
        }
    }

    public abstract class ComponentEngineInvoker
    {
        public delegate void ComponentEngineInvokerDelegate(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity);
        public class ComponentEngineInvokerDelegateSequenceGroup<TSequenceGroup>(bool sequence) : DelegateSequenceGroup<TSequenceGroup, ComponentEngineInvokerDelegate>(typeof(ComponentEngineInvokerDelegate), sequence)
            where TSequenceGroup : unmanaged, Enum
        {
            public void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
            {
                this.Sequenced?.Invoke(sourceEventId, entityTemplate, in entity);
            }
        }

        public abstract void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity);

        public static IEnumerable<ComponentEngineInvoker> Create<TSequenceGroup>(
            Type componentEngineInvokerType,
            Type engineType,
            IEnumerable<Type> componentTypes,
            IEnumerable<IEngine> engines,
            EntitiesDB entitiesDB,
            Func<Type, MethodInfo> method
        )
            where TSequenceGroup : unmanaged, Enum
        {
            Dictionary<ComponentEngineInvokerContext<TSequenceGroup>, List<IEngine>> validEngines = [];

            foreach (IEngine engine in engines)
            { // Iterate through all engines...
                foreach (Type onComponentEngineType in engine.GetType().GetConstructedGenericTypes(engineType))
                { // Select engines that specificaly implement the given engine interface...
                    if (componentTypes.Intersect(onComponentEngineType.GenericTypeArguments).Count() == onComponentEngineType.GenericTypeArguments.Length)
                    { // Only look at engine types that utilize the given list of components...
                        if (method(onComponentEngineType).TryGetSequenceGroup(engine, true, out SequenceGroup<TSequenceGroup> sequenceGroup) == true)
                        { // Only look at engines with a defined sequence group
                            ComponentEngineInvokerContext<TSequenceGroup> context = new(onComponentEngineType.GenericTypeArguments, sequenceGroup);
                            ref List<IEngine>? validEngineList = ref CollectionsMarshal.GetValueRefOrAddDefault(validEngines, context, out _);
                            validEngineList ??= [];

                            validEngineList.Add(engine);
                        }
                    }
                }
            }

            List<ComponentEngineInvoker> invokers = [];
            foreach (var (context, _) in validEngines)
            {
                Type invokerType = componentEngineInvokerType.MakeGenericType(context.Types);
                ComponentEngineInvoker invoker = (ComponentEngineInvoker)Activator.CreateInstance(invokerType, context.SequenceGroup, engines, entitiesDB)!;
                invokers.Add(invoker);
            }

            return invokers;
        }
    }

    public class OnSpawnEngineInvoker<TComponent>(SequenceGroup<OnSpawnSequenceGroupEnum> sequenceGroup, IEnumerable<IEngine> engines, EntitiesDB entitiesDB) : ComponentEngineInvoker, IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnSpawnSystem<TComponent>[] _engines = engines.OfType<IOnSpawnSystem<TComponent>>().ToArray();

        SequenceGroup<OnSpawnSequenceGroupEnum> IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            ref TComponent component = ref this._entitiesDB.QueryEntityByIndex<TComponent>(entity.Index, entity.Group);
            Entity<TComponent> entityC = new(in entity, ref component);

            foreach (IOnSpawnSystem<TComponent> engine in this._engines)
            {
                engine.OnSpawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }

    public class OnSpawnEngineInvoker<TComponent1, TComponent2>(SequenceGroup<OnSpawnSequenceGroupEnum> sequenceGroup, IEnumerable<IEngine> engines, EntitiesDB entitiesDB) : ComponentEngineInvoker, IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>
        where TComponent1 : unmanaged, IEntityComponent
        where TComponent2 : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnSpawnSystem<TComponent1, TComponent2>[] _engines = engines.OfType<IOnSpawnSystem<TComponent1, TComponent2>>().ToArray();

        SequenceGroup<OnSpawnSequenceGroupEnum> IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            var (component1s, component2s, _) = this._entitiesDB.QueryEntities<TComponent1, TComponent2>(entity.Group);
            Entity<TComponent1, TComponent2> entityC = new(in entity, ref component1s[entity.Index], ref component2s[entity.Index]);

            foreach (IOnSpawnSystem<TComponent1, TComponent2> engine in this._engines)
            {
                engine.OnSpawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }

    public class OnDespawnEngineInvoker<TComponent>(SequenceGroup<OnDespawnSequenceGroupEnum> sequenceGroup, IEnumerable<IEngine> engines, EntitiesDB entitiesDB) : ComponentEngineInvoker, IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnDespawnSystem<TComponent>[] _engines = engines.OfType<IOnDespawnSystem<TComponent>>().ToArray();

        SequenceGroup<OnDespawnSequenceGroupEnum> IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            ref TComponent component = ref this._entitiesDB.QueryEntityByIndex<TComponent>(entity.Index, entity.Group);
            Entity<TComponent> entityC = new(in entity, ref component);

            foreach (IOnDespawnSystem<TComponent> engine in this._engines)
            {
                engine.OnDespawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }

    public class OnDespawnEngineInvoker<TComponent1, TComponent2>(SequenceGroup<OnDespawnSequenceGroupEnum> sequenceGroup, IEnumerable<IEngine> engines, EntitiesDB entitiesDB) : ComponentEngineInvoker, IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>
        where TComponent1 : unmanaged, IEntityComponent
        where TComponent2 : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnDespawnSystem<TComponent1, TComponent2>[] _engines = engines.OfType<IOnDespawnSystem<TComponent1, TComponent2>>().ToArray();

        SequenceGroup<OnDespawnSequenceGroupEnum> IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>.Value { get; } = sequenceGroup;

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, in Entity entity)
        {
            var (component1s, component2s, _) = this._entitiesDB.QueryEntities<TComponent1, TComponent2>(entity.Group);
            Entity<TComponent1, TComponent2> entityC = new(in entity, ref component1s[entity.Index], ref component2s[entity.Index]);

            foreach (IOnDespawnSystem<TComponent1, TComponent2> engine in this._engines)
            {
                engine.OnDespawn(sourceEventId, entityTemplate, ref entityC);
            }
        }
    }
}