using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(RigidShipPart))]
    internal sealed class RigidShipPartClientDriver : Driver<RigidShipPart>
    {
        #region Private Fields
        private Queue<Fixture> _fixtures;
        private ServerShadow _server;
        #endregion

        #region Constructor
        public RigidShipPartClientDriver(ServerShadow shadow, RigidShipPart driven) : base(driven)
        {
            _server = shadow;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _fixtures = new Queue<Fixture>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<ShipPart.ChainUpdate>("chain:updated", this.HandleChainUpdated);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _fixtures.Clear();
        }
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ShipPart.ChainUpdate arg)
        {
            // If the chain update event is moving downwards, we should recreate the internal fixtures
            if (arg.HasFlag(ShipPart.ChainUpdate.Down))
                this.driven.AddFixturesToRoot(_server[this.driven.Root], _fixtures);
        }
        #endregion
    }
}
