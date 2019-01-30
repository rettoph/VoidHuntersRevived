using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Interfaces
{
    public interface IUserPlayerDriver : IEntity
    {
        void Read(NetIncomingMessage im);
        void Write(NetOutgoingMessage om);
    }
}
