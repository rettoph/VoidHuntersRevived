using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Extensions.Entities.WorldObjects
{
    public static class IWorldObjectExtensions
    {
        #region Network Methods
        public static void WriteTransformation(this IWorldObject worldObject, NetOutgoingMessage om)
        {
            om.Write(worldObject.Position);
            om.Write(worldObject.Rotation);
        }

        public static void ReadTransformation(this IWorldObject worldObject, NetIncomingMessage im, NetworkAuthorization authorization = NetworkAuthorization.Master)
        {
            worldObject.TrySetTransformation(
                im.ReadVector2(),
                im.ReadSingle(),
                authorization);
        }
        #endregion
    }
}
