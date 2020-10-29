using Guppy.Network.Groups;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Utilities
{
    public static class NetworkEntityMessageBuilder
    {
        public static NetOutgoingMessage BuildCreateMessage(Group group, NetworkEntity entity, NetConnection recipient = null)
            => group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, recipient).Write("entity:create", om =>
                {
                    om.Write(entity.ServiceDescriptor.Id);
                    om.Write(entity.Id);
                    entity.TryWrite(om);
                });

        public static NetOutgoingMessage BuildUpdateMessage(NetDeliveryMethod method, Int32 sequenceChannel, Group group, NetworkEntity entity, NetConnection recipient = null)
            => group.Messages.Create(method, sequenceChannel, recipient).Write("entity:update", om =>
                {
                    om.Write(entity.Id);
                });

        public static NetOutgoingMessage BuildRemoveMessage(Group group, NetworkEntity entity)
            => group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1).Write("entity:remove", om =>
                {
                    om.Write(entity.Id);
                });
    }
}
