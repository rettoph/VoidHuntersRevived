using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Utilities.Delegaters;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Extensions.Logging;
using Guppy.Network.Security;

namespace VoidHuntersRevived.Library.Utilities.Delegater
{
    /// <summary>
    /// Represents a new action message created by a NetworkEntity instance.
    /// </summary>
    public sealed class ActionDelegater : CustomDelegater<String, NetIncomingMessage>
    {
        private NetworkEntity _entity;
        private NetworkScene _scene;
        private ILogger _logger;

        public ActionDelegater(NetworkEntity networkEntity, NetworkScene scene, ILogger logger)
        {
            _entity = networkEntity;
            _scene = scene;
            _logger = logger;
        }

        public NetOutgoingMessage Create(String type, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.ReliableUnordered)
        {
            var om = _scene.Group.CreateMessage("entity:action", recipient, method, 0);
            om.Write(_entity.Id);
            om.Write(type);

            return om;
        }

        public NetOutgoingMessage Create(String type, User recipient, NetDeliveryMethod method = NetDeliveryMethod.ReliableUnordered)
        {
            var om = _scene.Group.CreateMessage("entity:action", recipient, method, 0);
            om.Write(_entity.Id);
            om.Write(type);

            return om;
        }

        public NetOutgoingMessage Create(String type, NetDeliveryMethod method = NetDeliveryMethod.ReliableUnordered, Int32 sequenceChannel = 0)
        {
            var om = _scene.Group.CreateMessage("entity:action", method, sequenceChannel);
            om.Write(_entity.Id);
            om.Write(type);

            return om;
        }

        protected override void Invoke<T>(object sender, string key, T arg)
        {
#if DEBUG
            try
            {
                _logger.LogTrace($"Action recieved: {key}");
                base.Invoke(sender, key, arg);
            }
            catch(KeyNotFoundException e)
            {
                _logger.LogWarning($"Unhandled action recieved by {_entity.GetType().Name}({_entity.Id}) => '{key}'");
            }
#else
            base.Invoke(sender, key, arg);
#endif
        }
    }
}
