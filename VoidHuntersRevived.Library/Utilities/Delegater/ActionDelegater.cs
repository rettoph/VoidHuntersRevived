using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy.Utilities.Delegaters;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;

namespace GalacticFighters.Library.Utilities.Delegater
{
    /// <summary>
    /// Represents a new action message created by a NetworkEntity instance.
    /// </summary>
    public sealed class ActionDelegater : CustomDelegater<String, NetIncomingMessage>
    {
        private NetworkEntity _entity;
        private NetworkScene _scene;

        public ActionDelegater(NetworkEntity networkEntity, NetworkScene scene)
        {
            _entity = networkEntity;
            _scene = scene;
        }

        public NetOutgoingMessage Create(String type)
        {
            var om = _scene.Group.CreateMessage("entity:action", NetDeliveryMethod.UnreliableSequenced, 0);
            om.Write(_entity.Id);
            om.Write(type);

            return om;
        }
    }
}
