using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(ShipPart))]
    internal sealed class ServerShipPartDriver : Driver<ShipPart>
    {
        private Single _flushedHealth;

        public ServerShipPartDriver(ShipPart driven) : base(driven)
        {
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
            if (this.driven.Health != _flushedHealth)
            {
                var action = this.driven.Actions.Create("health");
                action.Write(this.driven.Health);

                _flushedHealth = this.driven.Health;
            }
        }
    }
}
