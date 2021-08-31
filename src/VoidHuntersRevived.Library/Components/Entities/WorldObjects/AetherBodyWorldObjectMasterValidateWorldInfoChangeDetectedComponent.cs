using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    /// <summary>
    /// Detect when a <see cref="AetherBodyWorldObject.Body"/> is dirty and must
    /// be broadcasted.
    /// </summary>
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal class AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent : RemoteHostComponent<AetherBodyWorldObject>
    {
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.ValidateWorldInfoChangeDetected += this.HandleValidateWorldInfoChangeDetected;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.ValidateWorldInfoChangeDetected -= this.HandleValidateWorldInfoChangeDetected;
        }

        private bool HandleValidateWorldInfoChangeDetected(IWorldObject sender, GameTime args)
        {
            return true;
        }
    }
}
