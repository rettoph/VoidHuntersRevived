using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities
{
    public sealed class ShipPartConfiguration
    {
        private IDictionary<Type, IShipPart> _components;

        public readonly string Name;

        public ShipPartConfiguration(string name)
        {
            _components = new Dictionary<Type, IShipPart>();
            this.Name = name;
        }
    }
}
