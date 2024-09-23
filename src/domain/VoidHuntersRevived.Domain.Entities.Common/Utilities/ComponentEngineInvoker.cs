using Guppy.Core.Common.Extensions.System;
using Svelto.DataStructures;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    internal abstract class ComponentEngineInvoker
    {
        public abstract void Invoke(VhId sourceEventId, IEntityType type, EntitiesDB entitiesDB, EntityId id, GroupIndex groupIndex);

        public static bool Create(Type componentEngineInvokerType, Type engineType, Type componentType, IEnumerable<IEngine> engines, [MaybeNullWhen(false)] out ComponentEngineInvoker invoker)
        {
            List<IEngine> onComponentEngines = new List<IEngine>();

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
                invoker = null;
                return false;
            }

            Type invokerType = componentEngineInvokerType.MakeGenericType(componentType);
            invoker = (ComponentEngineInvoker)Activator.CreateInstance(invokerType, onComponentEngines)!;

            return true;
        }
    }

    internal class OnSpawnEngineInvoker<T> : ComponentEngineInvoker
        where T : unmanaged, IEntityComponent
    {
        private FasterList<IOnSpawnEngine<T>> _engines;

        public OnSpawnEngineInvoker(IEnumerable<IEngine> engines)
        {
            _engines = new FasterList<IOnSpawnEngine<T>>(engines.OfType<IOnSpawnEngine<T>>().ToList());
        }

        public override void Invoke(VhId sourceEventId, IEntityType type, EntitiesDB entitiesDB, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref entitiesDB.QueryEntityByIndex<T>(groupIndex.Index, groupIndex.GroupID);
            for (int i = 0; i < _engines.count; i++)
            {
                _engines[i].OnSpawn(sourceEventId, type, id, ref component, in groupIndex);
            }
        }
    }

    internal class OnDespawnEngineInvoker<T> : ComponentEngineInvoker
        where T : unmanaged, IEntityComponent
    {
        private FasterList<IOnDespawnEngine<T>> _engines;

        public OnDespawnEngineInvoker(IEnumerable<IEngine> engines)
        {
            _engines = new FasterList<IOnDespawnEngine<T>>(engines.OfType<IOnDespawnEngine<T>>().ToList());
        }

        public override void Invoke(VhId sourceEventId, IEntityType type, EntitiesDB entitiesDB, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref entitiesDB.QueryEntityByIndex<T>(groupIndex.Index, groupIndex.GroupID);
            for (int i = 0; i < _engines.count; i++)
            {
                _engines[i].OnDespawn(sourceEventId, type, id, ref component, in groupIndex);
            }
        }
    }
}
