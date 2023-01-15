using VoidHuntersRevived.Common.Entities.Configurations;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IShipPartConfigurationService
    {
        void Add(ShipPartConfiguration configuration);
        void Remove(string name);
        ShipPartConfiguration Get(string name);
        IEnumerable<ShipPartConfiguration> GetAll();
        IEnumerable<TComponent> GetAll<TComponent>()
            where TComponent : class, IShipPartComponentConfiguration;
    }
}
