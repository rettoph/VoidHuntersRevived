using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Scenes
{
    /// <summary>
    /// Default scene containing a single instance of a farseer world.
    /// </summary>
    public class WorldScene : NetworkScene
    {
        #region Protected Properties
        protected World world { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.world = provider.GetRequiredService<World>();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.layers.TryUpdate(gameTime);
        }
        #endregion
    }
}
