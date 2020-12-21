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
        protected override void Initialize(BodyEntity driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.DirtyState |= DirtyState.Filthy;

            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.driven.master.WritePosition;
        }

        protected override void Release(BodyEntity driven)
        {
            base.Release(driven);

            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.driven.master.WritePosition;
        }
        #endregion
    }
}
