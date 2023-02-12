using Guppy.Resources;
using Guppy.Resources.Attributes;
using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Common.Entities.ShipParts
{
    public sealed partial class ShipPartResource : Resource<IDictionary<Type, IShipPartComponentConfiguration>, IEnumerable<IShipPartComponentConfiguration>>, IEnumerable<IShipPartComponentConfiguration>
    {
        private Dictionary<Type, IShipPartComponentConfiguration> _components = new();
        private Action<Entity>? _attachers;

        public override IDictionary<Type, IShipPartComponentConfiguration> Value
        {
            get => _components;
            set
            {
                this.RemoveRange(_components.Values.ToList());

                if(value is not null)
                {
                    this.AddRange(value.Values);
                }
            }
        }

        public ShipPartResource(string name, params IShipPartComponentConfiguration[] components) : base(name)
        {
            this.AddRange(components);
        }

        public void Add<TComponent>(TComponent component) 
            where TComponent : class, IShipPartComponentConfiguration
        {
            _components.Add(typeof(TComponent), component);
            _attachers += component.AttachComponentToEntity;
        }

        public void AddRange(IEnumerable<IShipPartComponentConfiguration> components)
        {
            foreach(var component in components)
            {
                _components.Add(component.GetType(), component);
                _attachers += component.AttachComponentToEntity;
            }

        }

        public void Remove<TComponent>()
            where TComponent : class, IShipPartComponentConfiguration
        {
            if(_components.Remove(typeof(TComponent), out var component))
            {
                _attachers -= component.AttachComponentToEntity;
            }
        }

        public void RemoveRange(IEnumerable<IShipPartComponentConfiguration> components)
        {
            foreach (var component in components)
            {
                if(_components.Remove(component.GetType(), out var removed))
                {
                    if(removed != component)
                    {
                        _components.Add(removed.GetType(), removed);
                        continue;
                    }

                    _attachers -= component.AttachComponentToEntity;
                }
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

        public override void Initialize(string path, IServiceProvider services)
        {
            foreach(IShipPartComponentConfiguration component in _components.Values)
            {
                component.Initialize(path, services);
            }
        }

        public override IEnumerable<IShipPartComponentConfiguration> GetJson()
        {
            return _components.Values;
        }

        public override void Export(string path, IServiceProvider services)
        {
            // throw new NotImplementedException();
        }
    }
}
