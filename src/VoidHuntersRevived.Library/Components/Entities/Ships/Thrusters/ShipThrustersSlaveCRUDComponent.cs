using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.DependencyInjection;
using Guppy.Network.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipThrustersSlaveCRUDComponent : ShipThrustersComponent
    {
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Messages.Ship.DirectionChanged].OnRead += this.ReadDirectionChangedMessage;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.Messages[Messages.Ship.DirectionChanged].OnRead -= this.ReadDirectionChangedMessage;
        }

        private void ReadDirectionChangedMessage(MessageTypeManager sender, NetIncomingMessage args)
        {
            this.TrySetDirection(args.ReadEnum<Direction>(), args.ReadBoolean());
        }
    }
}
