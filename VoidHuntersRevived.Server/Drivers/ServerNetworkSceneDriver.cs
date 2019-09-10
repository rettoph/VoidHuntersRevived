using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Extensions.Collection;
using Guppy.Network.Security;

namespace GalacticFighters.Server.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    internal class ServerNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ServerNetworkSceneDriver(EntityCollection entities, NetworkScene driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Entities
        protected override void Initialize()
        {
            base.Initialize();

            _entities.Events.TryAdd<Entity>("added", this.HandleEntityAdded);
            _entities.Events.TryAdd<Entity>("removed", this.HandleEntityRemoved);

            this.driven.Group.Users.Events.TryAdd<User>("added", this.HandleUserAdded);
        }
        #endregion

        #region Message Methods
        private void CreateCreateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.CreateMessage("entity:create", recipient, NetDeliveryMethod.ReliableUnordered);
            message.Write(entity.Configuration.Handle);
            message.Write(entity.Id);

            this.CreateUpdateMessage(entity, recipient);
        }

        private void CreateUpdateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.CreateMessage("entity:update", recipient, NetDeliveryMethod.ReliableUnordered);
            entity.TryWrite(message);
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(object sender, Entity entity)
        {
            if (entity is NetworkEntity)
                this.CreateCreateMessage(entity as NetworkEntity);
        }

        private void HandleEntityRemoved(object sender, Entity entity)
        {
            if(entity is NetworkEntity)
            { // Broadcast the entity removal  to all clients
                var message = this.driven.Group.CreateMessage("entity:remove", NetDeliveryMethod.ReliableUnordered);
                message.Write(entity.Id);
            }
        }

        private void HandleUserAdded(object sender, User user)
        {
            this.driven.Group.CreateMessage("setup:start", user, NetDeliveryMethod.ReliableOrdered);

            _entities.ForEach(e =>
            { // Send all existing network entities to the new user
                if (e is NetworkEntity)
                    this.CreateCreateMessage(e as NetworkEntity, user);
            });

            this.driven.Group.CreateMessage("setup:end", user, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion
    }
}
