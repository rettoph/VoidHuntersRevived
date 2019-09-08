using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    public class ClientNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ClientNetworkSceneDriver(EntityCollection entities, NetworkScene driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods 
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Group.Messages.TryAdd("entity:create", this.HandleEntityCreateMessage);
            this.driven.Group.Messages.TryAdd("entity:remove", this.HandleEntityRemoveMessage);
        }
        #endregion

        #region Message Handlers
        private void HandleEntityCreateMessage(object sender, NetIncomingMessage arg)
        {
            _entities.Create<NetworkEntity>(arg.ReadString(), e =>
            {
                e.SetId(arg.ReadGuid());
                e.TryRead(arg);
            });
        }

        private void HandleEntityRemoveMessage(object sender, NetIncomingMessage arg)
        {
            // Remove the old entity
            _entities.GetById(arg.ReadGuid()).Dispose();
        }
        #endregion
    }
}
