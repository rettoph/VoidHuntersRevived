namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IShipPartConfigurationService
    {
        void Add(ShipPartConfiguration configuration);
        void Remove(string name);
        ShipPartConfiguration Get(string name);
    }
}
