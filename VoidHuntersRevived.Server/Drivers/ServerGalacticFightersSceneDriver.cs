using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Groups;
using Guppy.Network.Security;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Extensions.Collection;

namespace GalacticFighters.Server.Drivers
{
    [IsDriver(typeof(GalacticFightersWorldScene))]
    public class ServerGalacticFightersSceneDriver : Driver<GalacticFightersWorldScene>
    {
        #region Private Fields
        private EntityCollection _entities;
        private Group _group;
        #endregion

        #region Constructor
        public ServerGalacticFightersSceneDriver(EntityCollection entities, GalacticFightersWorldScene scene) : base(scene)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PostInitialize()
        {
            base.PostInitialize();

            _entities.Events.TryAdd<Entity>("added", this.HandleEntityAdded);
            _entities.Events.TryAdd<Entity>("removed", this.HandleEntityRemoved);

            _group = (this.driven as GalacticFightersWorldScene).Group;
            _group.Users.Events.TryAdd<User>("added", this.HandleUserJoined);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// If the new entity is a network entity, 
        /// send a creation message to all clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entity"></param>
        private void HandleEntityAdded(object sender, Entity entity)
        {
            if(entity is NetworkEntity)
                (entity as NetworkEntity).TryWrite(
                    _group.CreateMessage("entity:create", NetDeliveryMethod.ReliableOrdered, 0));
        }

        /// <summary>
        /// When a network entity is removed, we must
        /// broadcast a removal message to all connected
        /// clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entity"></param>
        private void HandleEntityRemoved(object sender, Entity entity)
        {
            if(entity is NetworkEntity)
                _group.CreateMessage("entity:create", NetDeliveryMethod.ReliableOrdered, 0)
                    .Write(entity.Id);
        }

        /// <summary>
        /// When a new user joins, broadcast all current entities to them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="user"></param>
        private void HandleUserJoined(object sender, User user)
        {
            _entities.ForEach(e =>
            {
                if(e is NetworkEntity)
                    (e as NetworkEntity).TryWrite(
                        _group.CreateMessage("entity:create", user, NetDeliveryMethod.ReliableOrdered, 0));
            });
        }
        #endregion
    }
}
