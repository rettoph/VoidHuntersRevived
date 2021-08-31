using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Services
{
    public class ShipService : Service
    {
        #region Private Fields
        private GuppyServiceProvider _provider;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _provider = provider;
        }

        protected override void Release()
        {
            base.Release();

            _provider = default;
        }
        #endregion

        public Ship Create(Chain chain, Player player = default)
        {
            return _provider.GetService<Ship>(Constants.ServiceConfigurationKeys.Ship, (ship, _, _) =>
            {
                ship.Chain = chain;
                ship.Player = player;
            });
        }
    }
}
