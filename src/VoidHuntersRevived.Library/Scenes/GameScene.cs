using Guppy;
using Guppy.DependencyInjection;
using Guppy.LayerGroups;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Utilities;
using log4net;
using System.Linq;
using Guppy.Extensions.System;
using Guppy.Enums;
using Guppy.Utilities;
using Guppy.Interfaces;

namespace VoidHuntersRevived.Library.Scenes
{
    public partial class GameScene : Scene
    {
        #region Private Fields
        private WorldEntity _world;
        private Action<WorldEntity> _onWorldActions;
        private ActionTimer _dirtyEntityCleanTimer;
        private NetworkEntity _entity;
        #endregion

        #region Protected Attributes
        protected Settings settings { get; private set; }
        protected Group group { get; private set; }
        #endregion

        #region Internal Fields
        internal Queue<NetworkEntity> dirtyEntities { get; set; }
        #endregion

        #region Public Attributes
        public Group Group => this.group;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _world = default(WorldEntity);
            _onWorldActions = default(Action<WorldEntity>);
            _dirtyEntityCleanTimer = new ActionTimer(100);
            this.dirtyEntities = new Queue<NetworkEntity>();

            this.settings = provider.GetService<Settings>();

            this.group = provider.GetService<Peer>()?.Groups?.GetOrCreateById(Guid.Empty);

            #region Layers
            // Create some default layers.
            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.World);
            });

            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.Player);
            });

            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.Chunk);
            });

            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.Trail);
            });

            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.Ship);
            });

            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.TractorBeam);
            });

            this.Layers.Create<ExplosionLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.Explosion);
            });

            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.SetContext(VHR.LayersContexts.Ammunition);
            });
            #endregion

            this.Entities.OnAdded += this.HandleEntityAdded;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            if (this.settings.Get<HostType>() == HostType.Remote)
                this.OnUpdate += this.UpdateRemote;
            else
                this.OnUpdate += this.UpdateLocal;
        }

        protected override void Release()
        {
            base.Release();

            this.Entities.OnAdded -= this.HandleEntityAdded;
            this.OnUpdate -= this.UpdateRemote;
            this.OnUpdate -= this.UpdateLocal;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private void UpdateRemote(GameTime gameTime)
        {
            this.group.TryUpdate(gameTime);

            _dirtyEntityCleanTimer.Update(gameTime, gt =>
            { // Flush all dirty entities down the peer as needed.
                while (this.dirtyEntities.Any())
                { // Attempt to clean all dirty entities as needed...
                    if((_entity = this.dirtyEntities.Dequeue()).Status == ServiceStatus.Ready)
                    {
                        this.group.Messages.Create(NetDeliveryMethod.Unreliable, 0).Then(om =>
                        { // Build a new update message...
                            _entity.MessageHandlers[MessageType.Update].TryWrite(om);
                        });
                    }
                }
            });
        }

        private void UpdateLocal(GameTime gameTime)
        {
            this.group?.Clear();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Preform an action immidiately if a world instance exists,
        /// or wait until that takes place then preform.
        /// </summary>
        /// <param name="action"></param>
        public void IfOrOnWorld(Action<WorldEntity> action)
        {
            if(_world == default(WorldEntity))
                _onWorldActions += action;
            else
                action(_world);
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(IEnumerable<IEntity> sender, IEntity arg)
        {
            if(arg is WorldEntity world)
            {
                this.Entities.OnAdded -= this.HandleEntityAdded;

                _world = world;
                _onWorldActions?.Invoke(_world);
            }
        }
        #endregion
    }
}
