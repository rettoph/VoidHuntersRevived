using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using Guppy.Extensions.DependencyInjection;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using Guppy.Network;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class Player : NetworkLayerable
    {
        #region Private Fields
        private Ship _ship;
        #endregion

        #region Public Properties
        public Ship Ship
        {
            get => _ship;
            set
            {
                this.OnShipChanged.InvokeIf(_ship != value, this, ref _ship, value);

                if (value != default && value.Player != this)
                    value.Player = this;
            }
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Player, Ship> OnShipChanged;
        #endregion

        #region Lifecycle Methods
        protected override void Create(GuppyServiceProvider provider)
        {
            base.Create(provider);

            this.LayerGroup = LayersContexts.Players.Group.GetValue();
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.GetService<PlayerService>().TryAdd(this);
        }
        #endregion
    }
}
