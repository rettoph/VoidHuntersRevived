using Guppy;
using Guppy.Network.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Players
{
    /// <summary>
    /// Players represent objects that can preform actions within ships.
    /// A player will always contain a ship
    /// </summary>
    public abstract class Player : Orderable, INetworkObject
    {
        public abstract String Name { get; }

        public void TryRead(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected virtual void Read(NetIncomingMessage im)
        {
            //
        }

        public void TryWrite(NetOutgoingMessage om)
        {
            throw new NotImplementedException();
        }

        protected virtual void Write(NetOutgoingMessage om)
        {
            // 
        }
    }
}
