using Guppy;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    /// <summary>
    /// Simple driver used to automatically invoke <see cref="IWorldObject.TryValidateWorldInfoChanged(GameTime)"/>
    /// every frame.
    /// </summary>
    internal sealed class WorldObjectCleanWorldInfoComponent : Component<IWorldObject>
    {
        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.OnPostUpdate += this.PostUpdate;
        }

        protected override void Release()
        {
            base.Release();

            this.Entity.OnPostUpdate -= this.PostUpdate;
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
            => this.Entity.TryValidateWorldInfoChanged(gameTime);
        #endregion
    }
}
