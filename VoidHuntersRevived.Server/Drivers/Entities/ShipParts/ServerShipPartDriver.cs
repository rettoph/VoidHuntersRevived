using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Server.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(ShipPart))]
    internal sealed class ServerShipPartDriver : Driver<ShipPart>
    {
        private Single _flushedHealth;
        private Interval _interval;

        public ServerShipPartDriver(Interval interval, ShipPart driven) : base(driven)
        {
            _interval = interval;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.TrySendHealth();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.TrySendHealth();
        }

        private void TrySendHealth()
        {
            if (_interval.Is(250) && this.driven.Health != _flushedHealth)
            {
                var action = this.driven.Actions.Create("health", NetDeliveryMethod.ReliableOrdered, 4);
                action.Write(this.driven.Health);

                _flushedHealth = this.driven.Health;
            }
        }
    }
}
