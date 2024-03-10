using Svelto.DataStructures;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    internal abstract class ComponentEngineInvoker
    {
        public abstract void Invoke(VhId sourceEventId, EntitiesDB entitiesDB, EntityId id, GroupIndex groupIndex);

        public static bool Create(Type componentEngineInvokerType, Type engineType, Type componentType, IEnumerable<IEngine> engines, [MaybeNullWhen(false)] out ComponentEngineInvoker invoker)
        {
            List<IEngine> onDespawnEngines = new List<IEngine>();

            foreach (IEngine engine in engines)
            {
                foreach (Type onDespawnEngineType in engine.GetType().GetConstructedGenericTypes(engineType))
                {
                    if (componentType == onDespawnEngineType.GenericTypeArguments[0])
                    {
                        onDespawnEngines.Add(engine);
                        continue;
                    }
                }
            }

            if (onDespawnEngines.Count == 0)
            {
                invoker = null;
                return false;
            }

            Type invokerType = componentEngineInvokerType.MakeGenericType(componentType);
            invoker = (ComponentEngineInvoker)Activator.CreateInstance(invokerType, onDespawnEngines)!;

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

        public override void Invoke(VhId sourceEventId, EntitiesDB entitiesDB, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref entitiesDB.QueryEntityByIndex<T>(groupIndex.Index, groupIndex.GroupID);
            for (int i = 0; i < _engines.count; i++)
            {
                _engines[i].OnSpawn(sourceEventId, id, ref component, in groupIndex);
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

        public override void Invoke(VhId sourceEventId, EntitiesDB entitiesDB, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref entitiesDB.QueryEntityByIndex<T>(groupIndex.Index, groupIndex.GroupID);
            for (int i = 0; i < _engines.count; i++)
            {
                _engines[i].OnDespawn(sourceEventId, id, ref component, in groupIndex);
            }
        }
    }
}
