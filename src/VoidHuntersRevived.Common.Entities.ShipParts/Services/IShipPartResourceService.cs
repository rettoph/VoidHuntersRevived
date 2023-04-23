using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface IShipPartResourceService
    {
        void Add(ShipPartResource configuration);
        void Remove(string name);
        ShipPartResource Get(string name);
        IEnumerable<ShipPartResource> GetAll();
        IEnumerable<TComponent> GetAll<TComponent>()
            where TComponent : class, IShipPartComponentConfiguration;
    }
}
