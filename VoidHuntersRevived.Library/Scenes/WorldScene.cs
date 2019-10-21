﻿using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Entities.Players;
using Guppy;
using Guppy.Collections;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Scenes
{
    public abstract class WorldScene : NetworkScene
    {
        #region Protected Fields
        protected World world { get; private set; }
        protected ChunkCollection chunks { get; private set; }
        #endregion

        #region Public Attributes
        public CreatableCollection<Player> Players { get; private set; }
        #endregion

        #region Constructor
        public WorldScene(World world)
        {
            this.world = world;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Create a new player collection...
            this.Players = new CreatableCollection<Player>(provider);

            this.chunks = provider.GetRequiredService<ChunkCollection>();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.chunks.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}
