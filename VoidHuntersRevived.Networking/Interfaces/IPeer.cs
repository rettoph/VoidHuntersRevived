using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface IPeer : IGroup
    {
        void Start();
    }
}
