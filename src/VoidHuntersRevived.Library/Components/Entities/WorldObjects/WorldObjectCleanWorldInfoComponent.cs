﻿using Guppy;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    /// <summary>
    /// Simple driver used to automatically invoke <see cref="IWorldObject.TryValidateWorldInfoDirty(GameTime)"/>
    /// every frame.
    /// </summary>
    internal sealed class WorldObjectCleanWorldInfoComponent : Component<IWorldObject>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.OnPostUpdate += this.PostUpdate;
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            this.Entity.OnPostUpdate -= this.PostUpdate;
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
            => this.Entity.TryValidateWorldInfoDirty(gameTime);
        #endregion
    }
}
