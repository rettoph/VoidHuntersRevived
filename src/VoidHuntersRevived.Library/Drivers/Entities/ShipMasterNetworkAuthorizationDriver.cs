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
        private ActionTimer _broadcastPingTimer;
        private Boolean _dirtyEnergy;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _explosions);

            this.driven.OnBridgeChanged += this.Health_HandleBridgeChanged;

            this.CleanBridge(default, this.driven.Bridge);
        }

        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            _broadcastPingTimer = new ActionTimer(150);

            this.driven.OnUpdate += this.Update;
            this.driven.OnBridgeChanged += this.HandleBridgeChanged;
            this.driven.OnFiringChanged += this.HandleFiringChanged;
            this.driven.OnDirectionChanged += this.HandleDirectionChanged;
            this.driven.TractorBeam.OnAction += this.HandleTractorBeamAction;
            this.driven.OnEnergyChanged += this.HandleEnergyChanged;
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);

            _explosions = null;

            this.driven.OnBridgeChanged -= this.Health_HandleBridgeChanged;
            this.CleanBridge(this.driven.Bridge, default);
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            _broadcastPingTimer = null;

            this.driven.OnUpdate -= this.Update;
            this.driven.OnBridgeChanged -= this.HandleBridgeChanged;
            this.driven.OnFiringChanged -= this.HandleFiringChanged;
            this.driven.OnDirectionChanged -= this.HandleDirectionChanged;
            this.driven.TractorBeam.OnAction -= this.HandleTractorBeamAction;
            this.driven.OnEnergyChanged -= this.HandleEnergyChanged;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            _broadcastPingTimer.Update(gameTime, gt =>
            { // Attempt to send the newest target value...
                this.WriteUpdateTarget(this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0));

                if(_dirtyEnergy)
                {
                    this.WriteUpdateEnergy(this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0));
                    _dirtyEnergy = false;
                }
            });
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
        private void WriteUpdateTarget(NetOutgoingMessage om)
            => om.Write(VHR.Pings.Ship.UpdateTarget, m =>
            {
                this.driven.WriteTarget(m);
            });

        private void WriteUpdateFiring(NetOutgoingMessage om, Boolean value)
            => om.Write(VHR.Pings.Ship.UpdateFiring, m =>
            {
                m.Write(value);
            });

        private void WriteUpdateDirection(NetOutgoingMessage om, Ship.Direction direction)
            => om.Write(VHR.Pings.Ship.UpdateDirection, m =>
            {
                this.driven.WriteDirection(m, direction);
            });

        private void WriteTractorBeamAction(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write(VHR.Pings.Ship.TractorBeam.Action, m =>
            {
                this.driven.TractorBeam.WriteAction(m, action);
            });
        }

        private void WriteUpdateBridge(NetOutgoingMessage om)
        {
            om.Write(VHR.Pings.Ship.UpdateBridge, m =>
            {
                this.driven.WriteBridge(om);
            });
        }

        private void WriteUpdateEnergy(NetOutgoingMessage om)
        {
            om.Write(VHR.Pings.Ship.UpdateEnergy, m =>
            {
                om.Write(this.driven.Energy);
                om.Write(this.driven.Charging);
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
                    MaxAge = 3f,
                    MaxDamagePerSecond = 10,
                    MaxForcePerSecond = 100,
                    MaxRadius = 40f
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

        private void HandleEnergyChanged(Ship sender, float old, float value)
            => _dirtyEnergy = true;
        #endregion
    }
}
