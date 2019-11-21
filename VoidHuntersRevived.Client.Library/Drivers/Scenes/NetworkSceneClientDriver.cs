using Guppy;
using Guppy.Attributes;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Extensions.Collections.Concurrent;
using Microsoft.Xna.Framework;
using System.Linq;
using Guppy.Collections;
using VoidHuntersRevived.Library.Entities;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Client.Library.Drivers.Scenes
{
    /// <summary>
    /// Manage the parsing of incoming create, update, &
    /// remove messages sent from the server.
    /// </summary>
    [IsDriver(typeof(NetworkScene))]
    internal sealed class NetworkSceneClientDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private Boolean _setup;
        private ConcurrentQueue<NetIncomingMessage> _creates;
        private ConcurrentQueue<NetIncomingMessage> _updates;
        private ConcurrentQueue<Guid> _removes;

        private NetIncomingMessage _im;
        private Guid _id;

        private EntityCollection _entities;
        #endregion

        #region Constructor
        public NetworkSceneClientDriver(EntityCollection entities, NetworkScene driven) : base(driven)
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
            _removes = new ConcurrentQueue<Guid>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Group.Messages.TryAdd("setup:start", (s, im) => _setup = false);
            this.driven.Group.Messages.TryAdd("setup:end", (s, im) => _setup = true);
            this.driven.Group.Messages.TryAdd("entity:create", (s, im) => _creates.Enqueue(im));
            this.driven.Group.Messages.TryAdd("entity:update", (s, im) => _updates.Enqueue(im));
            this.driven.Group.Messages.TryAdd("entity:remove", (s, im) => _removes.Enqueue(im.ReadGuid()));
        }

        protected override void Dispose()
        {
            base.Dispose();

            _creates.Clear();
            _updates.Clear();
            _removes.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_setup)
            {
                while (_creates.Any())
                    if (_creates.TryDequeue(out _im))
                        this.HandleCreateMessage(_im);

                while (_updates.Any())
                    if (_updates.TryDequeue(out _im))
                        this.HandleUpdateMessage(_im);

                while (_removes.Any())
                    if (_removes.TryDequeue(out _id))
                        this.HandleRemoveMessage(_id);
            }
        }
        #endregion

        #region Helper Methods
        private void HandleCreateMessage(NetIncomingMessage im)
        {
            var type = _im.ReadString();
            var id = _im.ReadGuid();

            if (_entities.GetById(id) == default(Entity))
            {
                _entities.Create<NetworkEntity>(type, e =>
                { // Create a new entity
                    e.SetId(id);
                    e.TryReadSetup(_im);
                });
            }
            else
                this.logger.LogWarning($"Recieved duplicate create messages for '{id}' => {id}");
        }

        private void HandleUpdateMessage(NetIncomingMessage im)
        {
            im.ReadEntity<NetworkEntity>(_entities)?.TryRead(_im);
        }

        private void HandleRemoveMessage(Guid id)
        {
            _entities.GetById(_im.ReadGuid())?.Dispose();
        }
        #endregion
    }
}
