﻿using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// Handle the fighter bay action request that can be invoked by a ship.
    /// </summary>
    internal sealed class ShipFighterBayDriver : Driver<Ship>
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _synchronizer);

            this.driven.Actions.Add(VHR.Actions.Ship.TryLaunchFighters, this.HandleTryLaunchFightersAction);
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);
        }
        #endregion

        #region Event Handlers
        private void HandleTryLaunchFightersAction(Ship sender, object args)
        {
            foreach (ShipPart shipPart in this.driven.Bridge.Items())
                if (shipPart is FighterBay fighterBay)
                    fighterBay.TryLaunch();

            // Invoke the post launch event...
            this.driven.Actions.TryInvoke(VHR.Actions.Ship.OnLaunchFighters);
        }
        #endregion
    }
}
