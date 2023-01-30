using Guppy.Resources;
using Guppy.Resources.Serialization.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json.Converters
{
    internal sealed class ShipPartResourceConverter : ResourceConverter<IDictionary<Type, IShipPartComponentConfiguration>, IEnumerable<IShipPartComponentConfiguration>>
    {
        public override IResource<IDictionary<Type, IShipPartComponentConfiguration>, IEnumerable<IShipPartComponentConfiguration>> Factory(string name, IEnumerable<IShipPartComponentConfiguration> json)
        {
            return new ShipPartResource(name, json.ToArray());
        }
    }
}
