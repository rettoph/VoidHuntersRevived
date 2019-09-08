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

            _entities.Events.TryAdd<Entity>("added", this.HandleAdded);
            _entities.Events.TryAdd<Entity>("removed", this.HandleRemoved);

            this.driven.Group.Users.Events.TryAdd<User>("added", this.HandleUserAdded);
        }
        #endregion

        #region Event Handlers
        private void HandleAdded(object sender, Entity arg)
        {
            if(arg is NetworkEntity)
            { // Broadcast the entity creation to all clients
                var message = this.driven.Group.CreateMessage("entity:create", NetDeliveryMethod.ReliableUnordered);
                message.Write(arg.Configuration.Handle);
                message.Write(arg.Id);
                (arg as NetworkEntity).TryWrite(message);
            }
        }

        private void HandleRemoved(object sender, Entity arg)
        {
            if(arg is NetworkEntity)
            { // Broadcast the entity removal  to all clients
                var message = this.driven.Group.CreateMessage("entity:remove", NetDeliveryMethod.ReliableUnordered);
                message.Write(arg.Id);
            }
        }

        private void HandleUserAdded(object sender, User arg)
        {
            // Send all existing network entities to the new user
            _entities.ForEach(e =>
            {
                if(e is NetworkEntity)
                {
                    var message = this.driven.Group.CreateMessage("entity:create", arg, NetDeliveryMethod.ReliableUnordered);
                    message.Write(e.Configuration.Handle);
                    (e as NetworkEntity).TryWrite(message);
                }
            });
        }
        #endregion
    }
}
