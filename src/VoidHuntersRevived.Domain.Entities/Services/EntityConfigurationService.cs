using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using static Guppy.Common.ThrowIf;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal class EntityConfigurationService : IEntityConfigurationService
    {
        private readonly EntityTypeService _types;
        private readonly EntityPropertyService _properties;
        private readonly Dictionary<EntityName, EntityConfiguration> _configurations;

        public EntityConfigurationService(
            EntityTypeService types, 
            EntityPropertyService properties,
            ISorted<IEntityLoader> loaders)
        {
            _types = types;
            _properties = properties;
            _configurations = new Dictionary<EntityName, EntityConfiguration>();

            foreach(IEntityLoader loader in loaders)
            {
                loader.Configure(this);
            }

            foreach(EntityConfiguration configuration in _configurations.Values)
            {
                configuration.Initialize(types, properties);
            }
        }

        public void Configure(EntityName name, Action<IEntityConfiguration> configuration)
        {
            configuration(this.GetOrCreateConfiguration(name));
        }

        internal EntityConfiguration GetOrCreateConfiguration(EntityName name)
        {
            if (!_configurations.TryGetValue(name, out EntityConfiguration? configuration))
            {
                _configurations[name] = configuration = new EntityConfiguration(name);
            }

            return configuration;
        }

        internal EntityConfiguration GetConfiguration(EntityName name)
        {
            return _configurations[name];
        }
    }
}
