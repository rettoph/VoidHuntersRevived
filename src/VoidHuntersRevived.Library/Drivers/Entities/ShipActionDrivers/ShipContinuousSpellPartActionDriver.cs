using Guppy.DependencyInjection;
using Guppy.Utilities;
using log4net;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers
{
    public abstract class ShipContinuousSpellPartActionDriver<TContinuousSpellPart> : ShipActionDriver<TContinuousSpellPart>
        where TContinuousSpellPart : ContinuousSpellPart
    {
        #region Private Fields
        private Boolean _active;
        private Synchronizer _synchronizer;
        private ILog _log;
        #endregion

        #region Protected Properties
        protected Boolean Active => _active;
        protected abstract Boolean DefaultActiveState { get; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _synchronizer);
            provider.Service(out _log);

            _active = this.DefaultActiveState;

            this.driven.OnClean += this.HandleClean;

            if (this.initialNetworkAuthorization == NetworkAuthorization.Master)
                this.driven.OnChargingChanged += this.HandleChargingChanged;
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);

            _synchronizer = null;

            this.driven.OnClean -= this.HandleClean;

            if (this.initialNetworkAuthorization == NetworkAuthorization.Master)
                this.driven.OnChargingChanged -= this.HandleChargingChanged;
        }
        #endregion

        #region Helper Methods
        protected override bool TryAction(IEnumerable<TContinuousSpellPart> parts, GameTime gameTime, ref Byte data)
        {
            var targetState = (ContinuousSpellPartTargetState)data;
            Boolean target = false;

            switch (targetState)
            {
                case ContinuousSpellPartTargetState.Inactive:
                    target = false;
                    break;
                case ContinuousSpellPartTargetState.Active:
                    target = true;
                    break;
                case ContinuousSpellPartTargetState.Toggle:
                    target = !_active;
                    data = target ? (Byte)ContinuousSpellPartTargetState.Active : (Byte)ContinuousSpellPartTargetState.Inactive;
                    _active = target;
                    break;
                case ContinuousSpellPartTargetState.Refresh:
                    target = _active;
                    break;
            }

            foreach (TContinuousSpellPart continuousSpellPart in parts)
            {
                if (target)
                {
                    continuousSpellPart.TryCast(gameTime);
                }
                else
                {
                    continuousSpellPart.StopCast();
                }
            }

            return true;
        }
        #endregion

        #region Event Handlers
        private void HandleChargingChanged(SpellCaster sender, bool old, bool value)
        {
            if (_active)
            {
                if (value)
                {
                    _synchronizer.Enqueue(gt => this.driven.Actions.TryInvoke(this.TryActionId, gt, (Byte)ContinuousSpellPartTargetState.Inactive));
                }
                else
                {
                    _synchronizer.Enqueue(gt => this.driven.Actions.TryInvoke(this.TryActionId, gt, (Byte)ContinuousSpellPartTargetState.Active));
                }
            }
        }

        private void HandleClean(Ship sender)
            => _synchronizer.Enqueue(gt => this.driven?.Actions.TryInvoke(this.TryActionId, gt, (Byte)ContinuousSpellPartTargetState.Refresh));
        #endregion
    }
}
