using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Interfaces
{
    public interface IFarseerEntity : IEntity, INetworkEntity
    {
        World World { get; }
        Body Body { get; }
    }
}
