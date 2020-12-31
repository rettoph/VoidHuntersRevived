using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
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
        private WorldEntity _world;
        private ActionTimer _targetSendTimer;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _entities);
            provider.Service(out _synchronizer);
            provider.Service(out _world);

            this.driven.OnBridgeChanged += this.Health_HandleBridgeChanged;

            this.CleanBridge(default, this.driven.Bridge);
        }

        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            _targetSendTimer = new ActionTimer(150);

            this.driven.OnUpdate += this.Update;
            this.driven.OnBridgeChanged += this.HandleBridgeChanged;
            this.driven.OnFiringChanged += this.HandleFiringChanged;
            this.driven.OnDirectionChanged += this.HandleDirectionChanged;
            this.driven.TractorBeam.OnAction += this.HandleTractorBeamAction;
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);

            _entities = null;
            _synchronizer = null;
            _world = null;

            this.driven.OnBridgeChanged -= this.Health_HandleBridgeChanged;
            this.CleanBridge(this.driven.Bridge, default);
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            _targetSendTimer = null;

            this.driven.OnUpdate -= this.Update;
            this.driven.OnBridgeChanged -= this.HandleBridgeChanged;
            this.driven.OnFiringChanged -= this.HandleFiringChanged;
            this.driven.OnDirectionChanged -= this.HandleDirectionChanged;
            this.driven.TractorBeam.OnAction -= this.HandleTractorBeamAction;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            _targetSendTimer.Update(gameTime, gt =>
            { // Attempt to send the newest target value...
                this.WriteUpdateTarget(this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 0));
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
            => om.Write("update:ship:target", m =>
            {
                this.driven.WriteTarget(m);
            });

        private void WriteUpdateFiring(NetOutgoingMessage om, Boolean value)
            => om.Write("update:ship:firing", m =>
            {
                m.Write(value);
            });

        private void WriteUpdateDirection(NetOutgoingMessage om, Ship.Direction direction)
            => om.Write("update:ship:direction", m =>
            {
                this.driven.WriteDirection(m, direction);
            });

        private void WriteTractorBeamAction(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write("ship:tractor-beam:action", m =>
            {
                this.driven.TractorBeam.WriteAction(m, action);
            });
        }

        private void WriteUpdateBridge(NetOutgoingMessage om, ShipPart bridge)
        {
            om.Write("update:ship:bridge", m =>
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
                // Release the part.
                _synchronizer.Enqueue(gt => sender.TryRelease());

                // Create an explosion on the bridge (this should kill the bridge)
                _world.EnqeueExplosion(sender.Position, new Color(sender.Chain.Ship.Color, 0.5f), 10f, 10f);

                // Just in case, manually remove bridge reference.
                this.driven.Bridge = default;
            }
        }

        private void HandleBridgeChanged(Ship sender, ShipPart old, ShipPart value)
            => this.WriteUpdateBridge(
                    this.driven.Actions.Create(NetDeliveryMethod.ReliableOrdered, 10),
                    value);

        private void HandleDirectionChanged(Ship sender, Ship.DirectionState args)
            => this.WriteUpdateDirection(
                this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 0), 
                args.Direction);

        private void HandleFiringChanged(Ship sender, bool value)
            => this.WriteUpdateFiring(
                    this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 0),
                    value);

        private void HandleTractorBeamAction(TractorBeam sender, TractorBeam.Action action)
            => this.WriteTractorBeamAction(
                this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 0),
                action);
        #endregion
    }
}
