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
using Guppy.Network.Utilitites.Delegaters;
using xxHashSharp;

namespace VoidHuntersRevived.Library.Utilities.Delegaters
{
    public class ActionMessageDelegater : HashedDelegater<NetIncomingMessage>
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
            om.Write(xxHash.CalculateHash(Encoding.UTF8.GetBytes(type)));

            return om;
        }

        public NetOutgoingMessage Create(String type, NetDeliveryMethod method, Int32 sequenceChannel, User recipient)
        {
            return this.Create(type, method, sequenceChannel, recipient.Connection);
        }
        #endregion

        protected override void Invoke<T>(object sender, UInt32 key, T arg)
        {
#if DEBUG
            try
            {
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
