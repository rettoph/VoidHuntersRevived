using Guppy.Resources.Providers;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ShipPartConfigurationService : IShipPartConfigurationService
    {
        private IDictionary<string, ShipPartResource> _configurations;

        public ShipPartConfigurationService(IResourceProvider resources)
        {
            _configurations = resources.GetAll<ShipPartResource>().ToDictionary(
                x => x.Name,
                x => x);
        }

        public void Add(ShipPartResource configuration)
        {
            _configurations.Add(configuration.Name, configuration);
        }

        public void Remove(string name)
        {
            _configurations.Remove(name);
        }
        public ShipPartResource Get(string name)
        {
            return _configurations[name];
        }

        public IEnumerable<ShipPartResource> GetAll()
        {
            return _configurations.Values;
        }

        IEnumerable<TComponent> IShipPartConfigurationService.GetAll<TComponent>()
        {
            return this.GetAll().SelectMany(x => x.OfType<TComponent>());
        }
    }
}
