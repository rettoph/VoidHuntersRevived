using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Common.Extensions.System.Reflection;
using Guppy.Core.Common.Interfaces;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    internal abstract class ComponentEngineInvoker
    {
        public abstract void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, EntityId id, GroupIndex groupIndex);

        public static IEnumerable<ComponentEngineInvoker> Create(Type componentEngineInvokerType, Type engineType, Type componentType, IEnumerable<IEngine> engines, EntitiesDB entitiesDB)
        {
            List<IEngine> onComponentEngines = [];

            foreach (IEngine engine in engines)
            {
                foreach (Type onComponentEngineType in engine.GetType().GetConstructedGenericTypes(engineType))
                {
                    if (componentType == onComponentEngineType.GenericTypeArguments[0])
                    {
                        onComponentEngines.Add(engine);
                        continue;
                    }
                }
            }

            if (onComponentEngines.Count == 0)
            {
                yield break;
            }

            Type invokerType = componentEngineInvokerType.MakeGenericType(componentType);

            foreach (IEngine onComponentEngine in onComponentEngines)
            {
                ComponentEngineInvoker invoker = (ComponentEngineInvoker)Activator.CreateInstance(invokerType, onComponentEngine, entitiesDB)!;
                yield return invoker;
            }
        }
    }

    internal class OnSpawnEngineInvoker<T>(IOnSpawnEngine<T> engine, EntitiesDB entitiesDB) : ComponentEngineInvoker, IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>
        where T : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnSpawnEngine<T> _engine = engine;

        SequenceGroup<OnSpawnSequenceGroupEnum> IRuntimeSequenceGroup<OnSpawnSequenceGroupEnum>.Value { get; } = engine.GetType()!.GetMethod(nameof(IOnSpawnEngine<T>.OnSpawn))!.TryGetSequenceGroup<OnSpawnSequenceGroupEnum>(engine, true, out var sequenceGroup)
            ? sequenceGroup : throw new NotImplementedException();

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref _entitiesDB.QueryEntityByIndex<T>(groupIndex.Index, groupIndex.GroupID);
            _engine.OnSpawn(sourceEventId, entityTemplate, id, ref component, in groupIndex);
        }
    }

    internal class OnDespawnEngineInvoker<T>(IOnDespawnEngine<T> engine, EntitiesDB entitiesDB) : ComponentEngineInvoker, IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>
        where T : unmanaged, IEntityComponent
    {
        private readonly EntitiesDB _entitiesDB = entitiesDB;
        private readonly IOnDespawnEngine<T> _engine = engine;

        SequenceGroup<OnDespawnSequenceGroupEnum> IRuntimeSequenceGroup<OnDespawnSequenceGroupEnum>.Value { get; } = engine.GetType()!.GetMethod(nameof(IOnDespawnEngine<T>.OnDespawn))!.TryGetSequenceGroup<OnDespawnSequenceGroupEnum>(engine, true, out var sequenceGroup)
            ? sequenceGroup : throw new NotImplementedException();

        public override void Invoke(VhId sourceEventId, IEntityTemplate entityTemplate, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref _entitiesDB.QueryEntityByIndex<T>(groupIndex.Index, groupIndex.GroupID);
            _engine.OnDespawn(sourceEventId, entityTemplate, id, ref component, in groupIndex);
        }
    }
}
