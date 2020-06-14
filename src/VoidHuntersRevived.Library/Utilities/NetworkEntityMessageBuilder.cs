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
                    om.Write(entity.Id);
                    om.Write(entity.ServiceConfiguration.Id);
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

        public static NetOutgoingMessage BuildSetupMessage(Group group, NetworkEntity entity, NetConnection recipient = null)
        {
            var om = NetworkEntityMessageBuilder.BuildUpdateMessage(NetDeliveryMethod.ReliableOrdered, 1, group, entity);
            entity.TryWrite(om);
            return om;
        }
    }
}
