using FarseerPhysics.Common;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.LayerGroups;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Scenes
{
    public class GameScene : Scene
    {
        #region Private Fields
        private WorldEntity _world;
        private Action<WorldEntity> _onWorldActions;
        #endregion

        #region Protected Attributes
        protected Settings settings { get; private set; }
        protected Logger logger { get; private set; }
        protected Group group { get; private set; }
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

            this.logger = provider.GetService<Logger>();

            this.group = provider.GetService<Peer>().Groups.GetOrCreateById(Guid.Empty);

            // Create some default layers.
            // World Components (ShipParts, WorldEntity, Players, ect)
            this.Layers.Create<GameLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(0);
            });

            this.Entities.OnAdded += this.HandleEntityAdded;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.group.TryDispose();

            this.Entities.OnAdded -= this.HandleEntityAdded;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.group.TryUpdate(gameTime);
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
        private void HandleEntityAdded(IEnumerable<Entity> sender, Entity arg)
        {
            if(arg is WorldEntity)
            {
                this.Entities.OnAdded -= this.HandleEntityAdded;

                _world = arg as WorldEntity;
                _onWorldActions?.Invoke(_world);
            }
        }
        #endregion
    }
}
