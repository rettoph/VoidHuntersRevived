using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    public class ClientNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;

        private ConcurrentQueue<NetIncomingMessage> _creates;
        private ConcurrentQueue<NetIncomingMessage> _updates;
        private ConcurrentQueue<NetIncomingMessage> _removes;

        private Boolean _setup;

        private NetIncomingMessage _im;

        private Guid _reservation;
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

            _creates = new ConcurrentQueue<NetIncomingMessage>();
            _updates = new ConcurrentQueue<NetIncomingMessage>();
            _removes = new ConcurrentQueue<NetIncomingMessage>();
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
                // Parse all create messages
                while(_creates.Any())
                    if(_creates.TryDequeue(out _im))
                    {
                        var type = _im.ReadString();
                        var id = _im.ReadGuid();

                        if (_entities.GetById(id) == default(Entity))
                        {
                            _entities.Create<NetworkEntity>(type, e =>
                            { // Create a new entity
                                e.SetId(id);
                                e.TryReadPreInitialize(_im);
                            }).TryReadPostInitialize(_im);
                        }
                        else
                            this.logger.LogWarning($"Recieved duplicate create messages for '{id}' => {id}");
                    }

                // Parse all update messages
                while (_updates.Any())
                    if(_updates.TryDequeue(out _im))
                        _entities.GetById<NetworkEntity>(_im.ReadGuid()).TryRead(_im);

                // Parse all remove messages
                while (_removes.Any())
                    if(_removes.TryDequeue(out _im))
                        _entities.GetById(_im.ReadGuid())?.Dispose();
            }
        }
        #endregion

        #region Message Handlers
        private void HandleEntityCreateMessage(object sender, NetIncomingMessage arg)
        {
            _creates.Enqueue(arg);
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
            this.logger.LogInformation($"Setup Start.");
        }

        private void HandleSetupEndMessage(object sender, NetIncomingMessage arg)
        {
            _setup = true;
            this.logger.LogInformation($"Setup End.");
        }
        #endregion
    }
}
