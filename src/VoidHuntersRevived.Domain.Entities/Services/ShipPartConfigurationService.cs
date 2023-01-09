﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ShipPartConfigurationService : IShipPartConfigurationService
    {
        private IDictionary<string, ShipPartConfiguration> _configurations;

        public ShipPartConfigurationService(IEnumerable<IShipPartConfigurationLoader> loaders)
        {
            _configurations = new Dictionary<string, ShipPartConfiguration>();

            foreach(IShipPartConfigurationLoader loader in loaders)
            {
                loader.Load(this);
            }
        }

        public void Add(ShipPartConfiguration configuration)
        {
            _configurations.Add(configuration.Name, configuration);
        }

        public void Remove(string name)
        {
            _configurations.Remove(name);
        }
        public ShipPartConfiguration Get(string name)
        {
            return _configurations[name];
        }

    }
}
