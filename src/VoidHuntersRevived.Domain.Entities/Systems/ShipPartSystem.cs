using Guppy.Common;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class ShipPartSystem : BasicSystem,
        ISubscriber<IEvent<CreateShipPart>>
    {
        private readonly IShipPartConfigurationService _shipPartsConfigurations;

        public ShipPartSystem(IShipPartConfigurationService shipPartsConfigurations)
        {
            _shipPartsConfigurations = shipPartsConfigurations;
        }

        public void Process(in IEvent<CreateShipPart> message)
        {
            var entity = message.Simulation.CreateEntity(message.Data.Key);
            var configuration = _shipPartsConfigurations.Get(message.Data.Configuration);

            configuration.Make(entity);
            entity.Attach(configuration);
            entity.Attach(new Jointings(entity));
        }
    }
}
