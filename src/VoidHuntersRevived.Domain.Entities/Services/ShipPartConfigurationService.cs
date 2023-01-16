using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;

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

        public IEnumerable<ShipPartConfiguration> GetAll()
        {
            return _configurations.Values;
        }

        IEnumerable<TComponent> IShipPartConfigurationService.GetAll<TComponent>()
        {
            return this.GetAll().SelectMany(x => x.WhereAs<IShipPartComponentConfiguration, TComponent>());
        }
    }
}
