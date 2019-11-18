using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library
{
    /// <summary>
    /// The client specific version of the BaseGame class
    /// </summary>
    public class ClientGame : BaseGame
    {
        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.scenes.Create<WorldScene>();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.scenes.TryDrawAll(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.scenes.TryUpdateAll(gameTime);
        }
        #endregion
    }
}
