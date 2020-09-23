using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Events;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ShipFullAuthorizationNetworkDriver : NetworkEntityNetworkDriver<Ship>
    {
        #region Private Fields
        private Boolean _dirtyTarget;
        private ActionTimer _targetSender;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            // Update the target a maximum of 4 times a second.
            _targetSender = new ActionTimer(100);

            this.driven.OnUpdate += this.Update;

            this.driven.OnWrite += this.WriteUpdateBridge;
            this.driven.OnWrite += this.WriteDirections;
            this.driven.OnWrite += this.WriteUpdateTarget;
            this.driven.OnBridgeChanged += this.HandleBridgeChanged;
            this.driven.TractorBeam.OnSelected += this.HandleTractorBeamSelected;
            this.driven.TractorBeam.OnDeselected += this.HandleTractorBeamDeselected;

            this.driven.Events[ShipEventType.Direction].OnEvent += this.HandleDirectionChanged;
            this.driven.Events[ShipEventType.Target].OnEvent += this.HandleTargetChanged;
        }

        protected override void DisposeFull()
        {
            base.DisposeFull();

            this.driven.OnUpdate -= this.Update;

            this.driven.OnWrite -= this.WriteUpdateBridge;
            this.driven.OnWrite -= this.WriteDirections;
            this.driven.OnWrite -= this.WriteUpdateTarget;
            this.driven.OnBridgeChanged -= this.HandleBridgeChanged;
            this.driven.TractorBeam.OnSelected -= this.HandleTractorBeamSelected;
            this.driven.TractorBeam.OnDeselected -= this.HandleTractorBeamDeselected;

            this.driven.Events[ShipEventType.Direction].OnEvent -= this.HandleDirectionChanged;
            this.driven.Events[ShipEventType.Target].OnEvent -= this.HandleTargetChanged;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            _targetSender.Update(gameTime, t => t && _dirtyTarget, () =>
            {
                this.WriteUpdateTarget(this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 4));
            });
        }
        #endregion

        #region Network Methods
        private void WriteUpdateBridge(NetOutgoingMessage om)
            => om.Write("update:bridge", m =>
            {
                om.Write(this.driven.Bridge);
            });

        private void WriteUpdateDirection(NetOutgoingMessage om, Ship.Direction direction, Boolean state)
            => om.Write("update:direction", m =>
            {
                om.Write((Byte)direction);
                om.Write(state);
            });

        private void WriteDirections(NetOutgoingMessage om)
        {
            // Write all current directions and their values...
            ((Ship.Direction[])Enum.GetValues(typeof(Ship.Direction))).ForEach(d =>
            {
                this.WriteUpdateDirection(om, d, this.driven.ActiveDirections.HasFlag(d));
            });
        }

        private void WriteUpdateTarget(NetOutgoingMessage om)
            => om.Write("update:target", m =>
            {
                om.Write(this.driven.Target);
            });

        private void WriteTractorBeamAction(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write("tractor-beam:action", m =>
            {
                m.Write(this.driven.TractorBeam.Position);
                m.Write((Byte)action.Type);
                m.Write(action.Target);
            });
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(Ship sender, ShipPart old, ShipPart value)
            => this.WriteUpdateBridge(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 4));

        private void HandleDirectionChanged(Ship ship, ShipEventArgs args)
            => this.WriteUpdateDirection(
                om: this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 4), 
                direction: args.DirectionData.Direction, 
                state: args.DirectionData.State);

        private void HandleTargetChanged(Ship ship, ShipEventArgs args)
            => _dirtyTarget = true;

        private void HandleTractorBeamSelected(TractorBeam sender, TractorBeam.Action action)
            => this.WriteTractorBeamAction(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 5), action);

        private void HandleTractorBeamDeselected(TractorBeam sender, TractorBeam.Action action)
            => this.WriteTractorBeamAction(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 5), action);
        #endregion
    }
}
