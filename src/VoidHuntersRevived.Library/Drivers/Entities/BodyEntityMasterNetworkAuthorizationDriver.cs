using Guppy;
using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Aether;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class BodyEntityMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<BodyEntity>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(BodyEntity driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.DirtyState |= DirtyState.Filthy;

            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.driven.master.WritePosition;
        }

        protected override void ReleaseRemote(BodyEntity driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.driven.master.WritePosition;
        }
        #endregion
    }
}
