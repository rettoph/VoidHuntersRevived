using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Lidgren.Network;
using log4net;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;
using Guppy.IO.Extensions.log4net;
using System.Linq;
using Guppy.Lists;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    internal sealed class GameSceneSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<GameScene>
    {
        #region Private Fields
        private ILog _logger;
        private EntityList _entities;

        private Queue<NetIncomingMessage> _creates;
        private Queue<NetIncomingMessage> _updates;
        private Queue<NetIncomingMessage> _removes;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GameScene driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _logger);
            provider.Service(out _entities);

            _creates = new Queue<NetIncomingMessage>();
            _updates = new Queue<NetIncomingMessage>();
            _removes = new Queue<NetIncomingMessage>();

            this.driven.Group.Messages.Set("scene:setup", this.HandleSceneSetupMessage);
            this.driven.Group.Messages.Set("entity:create", this.HandleEntityCreateMessage);
            this.driven.Group.Messages.Set("entity:update", this.HandleEntityUpdateMessage);
            this.driven.Group.Messages.Set("entity:remove", this.HandleEntityRemoveMessage);
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (_creates.Any()) // Flush all recieved create messages...
                this.CreateNetworkEntity(_creates.Dequeue());

            while (_updates.Any())
                this.UpdateNetworkEntity(_updates.Dequeue());

            while (_removes.Any())
                this.RemoveNetworkEntity(_removes.Dequeue());
        }
        #endregion

        #region Network Entity Manipulation Methods
        /// <summary>
        /// Recersively create all recieved NetworkEntities,
        /// then read network data for all entities, and
        /// finally complete initialization.
        /// </summary>
        /// <param name="im"></param>
        private void CreateNetworkEntity(NetIncomingMessage im)
            => _entities.Create<NetworkEntity>(descriptorId: im.ReadUInt32(), id: im.ReadGuid(), setup: (e, p, d) =>
                {
                    if (_creates.Any())
                        this.CreateNetworkEntity(_creates.Dequeue());

                    e.TryRead(im);
                });

        private void UpdateNetworkEntity(NetIncomingMessage im)
            => _entities.GetById<NetworkEntity>(im.ReadGuid()).TryRead(im);

        private void RemoveNetworkEntity(NetIncomingMessage im)
            => _entities.GetById<NetworkEntity>(im.ReadGuid()).TryRelease();
        #endregion

        #region Message Handlers
        private void HandleSceneSetupMessage(NetIncomingMessage obj)
        {
            if(obj.ReadBoolean())
            { // If setup is complete...
                _logger.Info(() => $"Recieved {_creates.Count()} NetworkEntities to create.");

                // do a first time update to flush the new messages...
                this.Update(new GameTime());
                this.driven.OnUpdate += this.Update;

                _logger.Info(() => $"Network setup completed.");
            }
            else
            {
                _logger.Info(() => "Network setup started.");
            }
        }

        private Boolean HandleEntityCreateMessage(NetIncomingMessage im)
        {
            _creates.Enqueue(im);
            return false;
        }

        private Boolean HandleEntityUpdateMessage(NetIncomingMessage im)
        {
            _updates.Enqueue(im);
            return false;
        }

        private Boolean HandleEntityRemoveMessage(NetIncomingMessage im)
        {
            _removes.Enqueue(im);
            return false;
        }
        #endregion
    }
}
