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
using Microsoft.Xna.Framework;
using System.Linq;

namespace GalacticFighters.Server.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    internal class ServerNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;
        private Queue<NetworkEntity> _created;
        #endregion

        #region Constructor
        public ServerNetworkSceneDriver(EntityCollection entities, NetworkScene driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Entities
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _created = new Queue<NetworkEntity>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _entities.Events.TryAdd<Entity>("added", this.HandleEntityAdded);
            _entities.Events.TryAdd<Entity>("removed", this.HandleEntityRemoved);

            this.driven.Group.Users.Events.TryAdd<User>("added", this.HandleUserAdded);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_created.Any())
                this.CreateCreateMessage(_created.Dequeue());
        }
        #endregion

        #region Message Methods
        private void CreateCreateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.CreateMessage("entity:create", recipient, NetDeliveryMethod.ReliableUnordered);
            message.Write(entity.Configuration.Handle);
            message.Write(entity.Id);
            entity.TryWriteSetup(message);

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
            if (entity is NetworkEntity) // Enqueue the new entity so the create message can be sent next frame
                _created.Enqueue(entity as NetworkEntity);
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

            _entities.Flush();
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
