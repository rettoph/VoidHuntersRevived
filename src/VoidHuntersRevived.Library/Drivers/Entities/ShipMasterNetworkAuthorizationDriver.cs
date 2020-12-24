﻿using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Lists;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// A simple driver that will primarily manage the <see cref="Ship.Bridge"/>s health and
    /// self destruct the <see cref="ShipPart"/> when <see cref="ShipPart.Health"/> reaches 0.
    /// </summary>
    internal sealed class ShipMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<Ship>
    {
        #region Private Fields
        private EntityList _entities;
        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _entities);
            provider.Service(out _synchronizer);

            this.driven.OnBridgeChanged += this.HandleBridgeChanged;
            this.CleanBridge(default, this.driven.Bridge);
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);

            this.driven.OnBridgeChanged -= this.HandleBridgeChanged;
            this.CleanBridge(this.driven.Bridge, default);
        }
        #endregion

        #region Helper Methods
        private void CleanBridge(ShipPart old, ShipPart bridge)
        {
            if(old != default)
            {
                old.OnHealthChanged -= this.HandleBridgeHealthChanged;
            }

            if(bridge != default)
            {
                bridge.OnHealthChanged += this.HandleBridgeHealthChanged;
            }
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(Ship sender, ShipPart old, ShipPart value)
            => this.CleanBridge(old, value);

        private void HandleBridgeHealthChanged(ShipPart sender, float old, float value)
        {
            if(sender.Health <= 0)
            { // Auto release the bridge

                // Create an explosion on the bridge (this should kill the bridge)
                _entities.Create<Explosion>((e, p, c) =>
                {
                    e.Position = sender.WorldCenter;
                    e.Velocity = sender.LinearVelocity;
                    e.Color = sender.Chain.Ship.Color;
                });

                // Just in case, manually release & delete it now
                this.driven.Bridge = default;
                _synchronizer.Enqueue(gt => sender.TryRelease());
            }
        }
        #endregion
    }
}
