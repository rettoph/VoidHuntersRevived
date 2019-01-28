using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface INetworkEntity : INetworkObject, IEntity
    {
        Boolean Dirty { get; set; }
        event EventHandler<INetworkEntity> OnDirty;
    }
}
