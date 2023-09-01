using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using static System.Formats.Asn1.AsnWriter;

namespace VoidHuntersRevived.Common.Entities.Utilities
{
    internal abstract class OnDespawnEngineInvoker
    {
        public abstract void Invoke(IEntityService entities, EntityId id, GroupIndex groupIndex);

        public static bool Create(Type componentType, IEnumerable<IEngine> engines, [MaybeNullWhen(false)] out OnDespawnEngineInvoker invoker)
        {
            List<IEngine> onDespawnEngines = new List<IEngine>();

            foreach (IEngine engine in engines)
            {
                foreach (Type onDespawnEngineType in engine.GetType().GetConstructedGenericTypes(typeof(IOnDespawnEngine<>)))
                {
                    if (componentType == onDespawnEngineType.GenericTypeArguments[0])
                    {
                        onDespawnEngines.Add(engine);
                        continue;
                    }
                }
            }

            if(onDespawnEngines.Count == 0)
            {
                invoker = null;
                return false;
            }

            Type invokerType = typeof(OnDespawnEngineInvoker<>).MakeGenericType(componentType);
            invoker = (OnDespawnEngineInvoker)Activator.CreateInstance(invokerType, onDespawnEngines)!;

            return true;
        }
    }

    internal class OnDespawnEngineInvoker<T> : OnDespawnEngineInvoker
        where T : unmanaged, IEntityComponent
    {
        private FasterList<IOnDespawnEngine<T>> _engines;

        public OnDespawnEngineInvoker(IEnumerable<IEngine> engines)
        {
            _engines = new FasterList<IOnDespawnEngine<T>>(engines.OfType<IOnDespawnEngine<T>>().ToList());
        }

        public override void Invoke(IEntityService entities, EntityId id, GroupIndex groupIndex)
        {
            ref T component = ref entities.QueryByGroupIndex<T>(groupIndex);

            for(int i=0; i<_engines.count; i++)
            {
                _engines[i].OnDespawn(id, ref component, in groupIndex);
            }
        }
    }
}
