using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities
{
    public sealed partial class ShipPartConfiguration
    {
        private IDictionary<Type, ComponentWrapper> _components;
        private Action<Entity>? _makers;
        public string Name { get; }

        public IEnumerable<IShipPartComponent> Components => _components.Values.Select(x => x.Component);

        public ShipPartConfiguration(string name)
        {
            _components = new Dictionary<Type, ComponentWrapper>();
            this.Name = name;
        }

        public void Add<TComponent>(TComponent component) 
            where TComponent : class, IShipPartComponent
        {
            var wrapper = new ComponentWrapper<TComponent>(component);
            _components.Add(typeof(TComponent), wrapper);
            _makers += wrapper.Maker;
        }

        public void Remove<TComponent>()
            where TComponent : class, IShipPartComponent
        {
            if(_components.Remove(typeof(TComponent), out var wrapper))
            {
                _makers -= wrapper.Maker;
            }
        }

        public void Make(Entity entity)
        {
            _makers?.Invoke(entity);
        }
    }
}
