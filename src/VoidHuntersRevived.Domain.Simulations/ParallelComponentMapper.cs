using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class ParallelComponentMapper<T> : IParallelComponentMapper<T>
        where T : class
    {
        private readonly ComponentMapper<T> _components;

        public ParallelComponentMapper(ComponentMapper<T> components)
        {
            _components = components;
        }

        public T Get(ParallelKey entityKey, ISimulation simulation)
        {
            return _components.Get(simulation.GetEntityId(entityKey));
        }

        public bool TryGet(ParallelKey entityKey, ISimulation simulation, [MaybeNullWhen(false)] out T value)
        {
            if(simulation.TryGetEntityId(entityKey, out int entityId))
            {
                return _components.TryGet(entityId, out value);
            }

            value = null;
            return false;
        }

        public bool Has(ParallelKey entityKey, ISimulation simulation)
        {
            if(simulation.TryGetEntityId(entityKey, out int entityId))
            {
                return _components.Has(entityId);
            }

            return false;
        }

        public void Put(ParallelKey entityKey, ISimulation simulation, T component)
        {
            _components.Put(simulation.GetEntityId(entityKey), component);
        }
    }
}
