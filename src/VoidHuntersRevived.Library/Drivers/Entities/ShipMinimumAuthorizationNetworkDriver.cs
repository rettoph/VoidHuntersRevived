using Guppy;
using Guppy.Collections;
using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO;
using log4net;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ShipMinimumAuthorizationNetworkDriver : NetworkEntityNetworkDriver<Ship>
    {
        #region Private Fields
        private EntityCollection _entities;
        private Vector2 _targetTarget;
        private ILog _logger;
        private Player _player;
        #endregion

        #region Static Fields
        public static Single TargetLerpStrength = 0.015625f;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            provider.Service(out _entities);
            provider.Service(out _logger);

            this.driven.OnUpdate += this.Update;

            this.driven.Actions.Set("update:bridge", this.ReadUpdateBridge);
            this.driven.Actions.Set("update:direction", this.ReadUpdateDirection);
            this.driven.Actions.Set("update:target", this.ReadUpdateTarget);
            this.driven.Actions.Set("tractor-beam:action", this.ReadTractorBeamAction);
        }

        protected override void DisposeMinimum()
        {
            base.DisposeMinimum();

            this.driven.OnUpdate -= this.Update;

            this.driven.Actions.Remove("update:bridge");
            this.driven.Actions.Remove("update:direction");
            this.driven.Actions.Remove("update:target");
            this.driven.Actions.Remove("tractor-beam:action");
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if(this.driven.Authorization == GameAuthorization.Local)
                this.driven.Target = Vector2.Lerp(this.driven.Target, _targetTarget, Math.Min(1, ShipMinimumAuthorizationNetworkDriver.TargetLerpStrength * (Single)gameTime.ElapsedGameTime.TotalMilliseconds));
        }
        #endregion

        #region Network Methods
        private void ReadUpdateBridge(NetIncomingMessage im)
            => this.driven.SetBridge(im.ReadEntity<ShipPart>(_entities));

        private void ReadUpdateDirection(NetIncomingMessage im)
            => this.driven.TrySetDirection((Ship.Direction)im.ReadByte(), im.ReadBoolean());

        private void ReadUpdateTarget(NetIncomingMessage im)
            => _targetTarget = im.ReadVector2();

        private void ReadTractorBeamAction(NetIncomingMessage im)
        {
            this.driven.TractorBeam.Position = im.ReadVector2();
            this.driven.TractorBeam.TryAction(new TractorBeam.Action((TractorBeam.ActionType)im.ReadByte(), im.ReadEntity<ShipPart>(_entities)));
        }
        #endregion
    }
}
