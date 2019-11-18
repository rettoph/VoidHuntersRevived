using Guppy.Utilities.Delegaters;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Security;

namespace VoidHuntersRevived.Library.Utilities.Delegaters
{
    public class ActionMessageDelegater : CustomDelegater<String, NetIncomingMessage>
    {
        #region Private Fields
        private NetworkEntity _entity;
        private NetworkScene _scene;
        private ILogger _logger;
        #endregion

        #region Constructor
        public ActionMessageDelegater(NetworkEntity networkEntity, NetworkScene scene, ILogger logger)
        {
            _entity = networkEntity;
            _scene = scene;
            _logger = logger;
        }
        #endregion

        #region Create Methods
        public NetOutgoingMessage Create(String type, NetDeliveryMethod method, Int32 sequenceChannel, NetConnection recipient = null)
        {
            var om = _scene.Group.Messages.Create("entity:action", method, sequenceChannel, recipient);
            om.Write(_entity.Id);
            om.Write(type);

            return om;
        }

        public NetOutgoingMessage Create(String type, NetDeliveryMethod method, Int32 sequenceChannel, User recipient)
        {
            return this.Create(type, method, sequenceChannel, recipient.Connection);
        }
        #endregion

        protected override void Invoke<T>(object sender, string key, T arg)
        {
#if DEBUG
            try
            {
                _logger.LogTrace($"Action recieved: {key}");
                base.Invoke(sender, key, arg);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning($"Unhandled action recieved by {_entity.GetType().Name}({_entity.Id}) => '{key}'");
            }
#else
            base.Invoke(sender, key, arg);
#endif
        }
    }
}
