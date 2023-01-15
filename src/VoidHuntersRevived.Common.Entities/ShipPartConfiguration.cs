using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Configurations;

namespace VoidHuntersRevived.Common.Entities
{
    public sealed partial class ShipPartConfiguration : IEnumerable<IShipPartComponentConfiguration>
    {
        private IDictionary<Type, IShipPartComponentConfiguration> _components;
        private Action<Entity>? _attachers;
        public string Name { get; }

        public ShipPartConfiguration(string name)
        {
            _components = new Dictionary<Type, IShipPartComponentConfiguration>();
            this.Name = name;
        }

        public void Add<TComponent>(TComponent component) 
            where TComponent : class, IShipPartComponentConfiguration
        {
            _components.Add(typeof(TComponent), component);
            _attachers += component.AttachComponentTo;
        }

        public void Remove<TComponent>()
            where TComponent : class, IShipPartComponentConfiguration
        {
            if(_components.Remove(typeof(TComponent), out var component))
            {
                _attachers -= component.AttachComponentTo;
            }
        }

        public void Make(Entity entity)
        {
            _attachers?.Invoke(entity);
        }

        public IEnumerator<IShipPartComponentConfiguration> GetEnumerator()
        {
            return _components.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
