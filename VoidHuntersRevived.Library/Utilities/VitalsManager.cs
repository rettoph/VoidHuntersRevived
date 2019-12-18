using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Utilities
{
    /// <summary>
    /// Simple class used to manage bulk vitals ping at a static rate.
    /// This will flush queued NetworkEntity Vital pings as needed.
    /// It will bottleneck any entities that claim to require excesive
    /// vital pings at a predefined rate and should group up multiple
    /// packets at once.
    /// </summary>
    public sealed class VitalsManager
    {
        #region Static Fields
        public static Int32 MessagesRecieved = 0;
        #endregion

        #region Private Fields
        private Group _group;
        private Queue<Action<NetOutgoingMessage>> _queue;
        private NetOutgoingMessage _om;
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public VitalsManager(EntityCollection entities)
        {
            _queue = new Queue<Action<NetOutgoingMessage>>();
            _entities = entities;
        }
        #endregion

        #region Helper Methods
        internal void SetGroup(Group group)
        {
            if (_group != default(Group))
                _group.Messages.TryRemove("vitals", this.HandleVitalsMessage);

            _group = group;
            _group?.Messages.TryAdd("vitals", this.HandleVitalsMessage);
        }

        public void Enqueue(Action<NetOutgoingMessage> write)
        {
            _queue.Enqueue(write);
        }

        /// <summary>
        /// Flush all enqueued entities.
        /// </summary>
        public void Flush()
        {
            while (_queue.Any())
                _queue.Dequeue().Invoke(_group.Messages.Create("vitals", NetDeliveryMethod.Unreliable, 2));
        }
        #endregion

        #region Message Handlers
        private void HandleVitalsMessage(object sender, NetIncomingMessage arg)
        {
            MessagesRecieved++;
            arg.ReadEntity<NetworkEntity>(_entities)?.TryReadVitals(arg);
        }
        #endregion
    }
}
