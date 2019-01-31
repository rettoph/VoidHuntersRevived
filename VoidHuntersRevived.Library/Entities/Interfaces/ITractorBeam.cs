using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities.Connections;

namespace VoidHuntersRevived.Library.Entities.Interfaces
{
    public interface ITractorBeam : IFarseerEntity
    {
        TractorBeamConnection Connection { get; }
        Vector2 Position { get; set; }

        event EventHandler<ITractorBeam> OnConnected;
        event EventHandler<ITractorBeam> OnDisconnected;

        void CreateConnection(ITractorableEntity target);
        void Connect(TractorBeamConnection connection);
        void Disconnect();

        void Read(NetIncomingMessage im);
        void Write(NetOutgoingMessage om);
    }
}
