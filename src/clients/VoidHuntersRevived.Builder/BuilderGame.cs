using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.Scenes;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Builder
{
    /// <summary>
    /// The default Guppy Game loaded within the project.
    /// </summary>
    public class BuilderGame : VoidHuntersRevivedGame
    {
        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Scenes.Create<BuilderScene>();
        }
        #endregion
    }
}
