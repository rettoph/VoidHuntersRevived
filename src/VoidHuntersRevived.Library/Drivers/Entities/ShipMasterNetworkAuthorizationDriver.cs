using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Lists;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// A simple driver that will primarily manage the <see cref="Ship.Bridge"/>s health and
    /// self destruct the <see cref="ShipPart"/> when <see cref="ShipPart.Health"/> reaches 0.
    /// </summary>
    internal sealed class ShipMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<Ship>
    {
        #region Private Fields
        private ExplosionService _explosions;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _explosions);

            this.driven.DirtyState |= DirtyState.Filthy;

            this.driven.OnBridgeChanged += this.Health_HandleBridgeChanged;

            this.CleanBridge(default, this.driven.Bridge);
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);

            _explosions = null;

            this.driven.OnBridgeChanged -= this.Health_HandleBridgeChanged;

            this.CleanBridge(this.driven.Bridge, default);
        }

        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.WriteUpdate;

            this.driven.OnBridgeChanged += this.HandleBridgeChanged;
            this.driven.OnFiringChanged += this.HandleFiringChanged;
            this.driven.OnDirectionChanged += this.HandleDirectionChanged;
            this.driven.TractorBeam.OnAction += this.HandleTractorBeamAction;
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.WriteUpdate;

            this.driven.OnBridgeChanged -= this.HandleBridgeChanged;
            this.driven.OnFiringChanged -= this.HandleFiringChanged;
            this.driven.OnDirectionChanged -= this.HandleDirectionChanged;
            this.driven.TractorBeam.OnAction -= this.HandleTractorBeamAction;
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

        #region Network Methods
        private void WriteUpdate(NetOutgoingMessage om)
        {
            this.driven.WriteTarget(om);
        }

        private void WriteUpdateFiring(NetOutgoingMessage om, Boolean value)
            => om.Write(VHR.Network.Pings.Ship.UpdateFiring, m =>
            {
                m.Write(value);
            });

        private void WriteUpdateDirection(NetOutgoingMessage om, Ship.Direction direction)
            => om.Write(VHR.Network.Pings.Ship.UpdateDirection, m =>
            {
                this.driven.WriteDirection(m, direction);
            });

        private void WriteTractorBeamAction(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write(VHR.Network.Pings.Ship.TractorBeam.Action, m =>
            {
                this.driven.TractorBeam.WriteAction(m, action);
            });
        }
        private void WriteUpdateBridge(NetOutgoingMessage om)
        {
            om.Write(VHR.Network.Pings.Ship.UpdateBridge, m =>
            {
                this.driven.WriteBridge(om);
            });
        }
        #endregion

        #region Event Handlers
        private void Health_HandleBridgeChanged(Ship sender, ShipPart old, ShipPart value)
            => this.CleanBridge(old, value);

        private void HandleBridgeHealthChanged(ShipPart sender, float old, float value)
        {
            if(sender.Health <= 0)
            { // Auto release the bridge
                _explosions.Create(new ExplosionContext()
                {
                    StartPosition = sender.Position,
                    StartVelocity = sender.LinearVelocity,
                    Color = sender.Chain?.Ship.Color ?? sender.Color,
                    MaxAge = 4f,
                    MaxDamagePerSecond = 10,
                    MaxForcePerSecond = 3000,
                    MaxRadius = 30f
                });

                // Release the part.
                sender.TryRelease();

                // Just in case, manually remove bridge reference.
                this.driven.Bridge = default;
            }
        }

        private void HandleBridgeChanged(Ship sender, ShipPart old, ShipPart value)
            => this.WriteUpdateBridge(
                    this.driven.Ping.Create(NetDeliveryMethod.ReliableOrdered, 10));

        private void HandleDirectionChanged(Ship sender, Ship.DirectionState args)
            => this.WriteUpdateDirection(
                this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0), 
                args.Direction);

        private void HandleFiringChanged(Ship sender, bool value)
            => this.WriteUpdateFiring(
                    this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0),
                    value);

        private void HandleTractorBeamAction(TractorBeam sender, TractorBeam.Action action)
            => this.WriteTractorBeamAction(
                this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0),
                action);
        #endregion
    }
}
