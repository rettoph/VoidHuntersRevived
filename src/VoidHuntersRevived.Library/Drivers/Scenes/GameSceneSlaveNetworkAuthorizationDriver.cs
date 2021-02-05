using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Lidgren.Network;
using log4net;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.log4net;
using System.Linq;
using Guppy.Lists;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Extensions.System;
using VoidHuntersRevived.Library.Enums;
using Guppy.Utilities;
using Guppy.Extensions.System.Collections;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    internal sealed class GameSceneSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<GameScene>
    {
        #region Private Fields
        private ILog _logger;
        private EntityList _entities;

        private Dictionary<MessageType, Queue<NetIncomingMessage>> _entityMessages;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(GameScene driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            provider.Service(out _logger);
            provider.Service(out _entities);

            _entityMessages = DictionaryHelper.BuildEnumDictionary<MessageType, Queue<NetIncomingMessage>>(t => new Queue<NetIncomingMessage>());

            this.driven.Group.Messages.Set(VHR.Pings.Scene.Setup, this.HandleSceneSetupMessage);
            this.driven.Group.Messages.Set(VHR.Pings.Scene.Entity, this.HandleEntityMessage);
        }

        protected override void ReleaseRemote(GameScene driven)
        {
            base.ReleaseRemote(driven);

            _logger = null;
            _entities = null;

            this.driven.Group.Messages.Remove(VHR.Pings.Scene.Setup);
            this.driven.Group.Messages.Remove(VHR.Pings.Scene.Entity);

            this.driven.OnUpdate -= this.Update;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            foreach(MessageType type in _entityMessages.Keys)
                while (_entityMessages[type].Any())
                    this.ReadEntityMessage(type, _entityMessages[type].Dequeue());
        }
        #endregion

        #region Network Entity Manipulation Methods
        private void ReadEntityMessage(MessageType type, NetIncomingMessage im)
        {
            switch (type)
            {
                case MessageType.Create:
                    var id = im.ReadGuid();
                    var descriptorId = im.ReadUInt32();

                    _entities.Create<NetworkEntity>(id: id, descriptorId: descriptorId, setup: (e, p, d) =>
                    {
                        e.MessageHandlers[MessageType.Create].TryRead(im);
                    });
                    break;
                case MessageType.Remove:
                    _entities.GetById<NetworkEntity>(im.ReadGuid()).Then(e =>
                    {
                        e.MessageHandlers[MessageType.Remove].TryRead(im);
                        e.TryRelease();
                    });
                    break;
                default:
                    _entities.GetById<NetworkEntity>(im.ReadGuid())?.MessageHandlers[type].TryRead(im);
                    break;
            }

            im.Recycle();
        }
        #endregion

        #region Message Handlers
        private void HandleSceneSetupMessage(NetIncomingMessage obj)
        {
            if(obj.ReadBoolean())
            { // If setup is complete...
                _logger.Info(() => $"Recieved {_entityMessages[MessageType.Create].Count()} NetworkEntities to create.");

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

        private Boolean HandleEntityMessage(NetIncomingMessage im)
        {
            _entityMessages[(MessageType)im.ReadByte()].Enqueue(im);
            return false;
        }
        #endregion
    }
}
