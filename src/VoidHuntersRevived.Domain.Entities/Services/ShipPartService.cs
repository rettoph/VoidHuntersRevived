using Guppy.Common;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ShipPartService : IShipPartService
    {
        private readonly IShipPartConfigurationService _configurations;

        public ShipPartService(IShipPartConfigurationService configurations)
        {
            _configurations = configurations;
        }

        public Entity Create(string configuration, ParallelKey key, ISimulation simulation)
        {
            var entity = simulation.CreateEntity(key);

            _configurations.Get(configuration).Make(entity);

            return entity;
        }
    }
}
