using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Utilities.ConnectionNodes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Library.Extensions.Lidgren
{
    public static class NetIncomingMessageExtensions
    {
        public static FemaleConnectionNode ReadFemaleConnectionNode(this NetIncomingMessage im, EntityCollection entities)
        {
            var targetId = im.ReadInt32();
            return im.ReadEntity<ShipPart>(entities)
                .FemaleConnectionNodes
                .First(f => f.Id == targetId);
        }

        public static MaleConnectionNode ReadMaleConnectionNode(this NetIncomingMessage im, EntityCollection entities)
        {
            return im.ReadEntity<ShipPart>(entities)
                .MaleConnectionNode;
        }
    }
}
