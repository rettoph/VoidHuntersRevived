using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    public class ClientNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;

        private Queue<NetIncomingMessage> _creates;
        private Queue<NetIncomingMessage> _updates;
        private Queue<NetIncomingMessage> _removes;

        private Boolean _setup;

        private NetIncomingMessage _im;
        #endregion

        #region Constructor
        public ClientNetworkSceneDriver(EntityCollection entities, NetworkScene driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods 
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _creates = new Queue<NetIncomingMessage>();
            _updates = new Queue<NetIncomingMessage>();
            _removes = new Queue<NetIncomingMessage>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _setup = false;

            this.driven.Group.Messages.TryAdd("setup:start", this.HandleSetupStartMessage);
            this.driven.Group.Messages.TryAdd("setup:end", this.HandleSetupEndMessage);
            this.driven.Group.Messages.TryAdd("entity:create", this.HandleEntityCreateMessage);
            this.driven.Group.Messages.TryAdd("entity:update", this.HandleEntityUpdateMessage);
            this.driven.Group.Messages.TryAdd("entity:remove", this.HandleEntityRemoveMessage);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.Group.Messages.TryRemove("setup:start", this.HandleSetupStartMessage);
            this.driven.Group.Messages.TryRemove("setup:end", this.HandleSetupEndMessage);
            this.driven.Group.Messages.TryRemove("entity:create", this.HandleEntityCreateMessage);
            this.driven.Group.Messages.TryRemove("entity:update", this.HandleEntityUpdateMessage);
            this.driven.Group.Messages.TryRemove("entity:remove", this.HandleEntityRemoveMessage);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_setup)
            {
                // Parse all update messages
                while (_updates.Any())
                    _entities.GetById<NetworkEntity>((_im = _updates.Dequeue()).ReadGuid()).TryRead(_im);

                // Parse all remove messages
                while(_removes.Any())
                    _entities.GetById(_removes.Dequeue().ReadGuid()).Dispose();
            }
        }
        #endregion

        #region Message Handlers
        private void HandleEntityCreateMessage(object sender, NetIncomingMessage arg)
        {
            var type = arg.ReadString();
            var id = arg.ReadGuid();

            if (_entities.GetById(id) == default(Entity))
                _entities.Create<NetworkEntity>(type, e =>
                { // Create a new entity
                    e.SetId(id);
                    e.TryReadSetup(arg);
                });
            else
                this.logger.LogWarning($"Recieved duplicate create messages for '{id}' => {id}");
        }

        private void HandleEntityUpdateMessage(object sender, NetIncomingMessage arg)
        {
            _updates.Enqueue(arg);
        }

        private void HandleEntityRemoveMessage(object sender, NetIncomingMessage arg)
        {
            _removes.Enqueue(arg);
        }

        private void HandleSetupStartMessage(object sender, NetIncomingMessage arg)
        {
            //
        }

        private void HandleSetupEndMessage(object sender, NetIncomingMessage arg)
        {
            _setup = true;
        }
        #endregion
    }
}
