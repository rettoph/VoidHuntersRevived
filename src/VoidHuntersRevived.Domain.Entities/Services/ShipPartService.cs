using Microsoft.Extensions.Configuration;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ShipPartService : IShipPartService
    {
        private readonly IShipPartResourceService _resources;

        public ShipPartService(IShipPartResourceService resources)
        {
            _resources = resources;
        }

        public void MakeShipPart(Entity entity, ShipPartResource resource)
        {
            resource.Make(entity);
            entity.Attach(resource);
        }

        public Entity CreateShipPart(ShipPartResource resource, ISimulationEvent @event)
        {
            Entity entity = @event.Simulation.CreateEntity(@event.NewKey());
            this.MakeShipPart(entity, resource);

            return entity;
        }

        public Entity CreateShipPart(string resourceName, ISimulationEvent @event)
        {
            ShipPartResource resource = _resources.Get(resourceName);

            return this.CreateShipPart(resource, @event);
        }
    }
}
