using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Services
{
    public class ShipService : Service
    {
        #region Private Fields
        private ServiceProvider _provider;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _provider = provider;
        }
        #endregion

        #region Helper Methods
        public Ship Create(Chain chain, Player player = default)
        {
            Ship ship = _provider.GetService<Ship>((ship, _, _) =>
            {
                ship.Chain = chain;
            });

            ship.Player = player;

            return ship;
        }
        #endregion
    }
}
