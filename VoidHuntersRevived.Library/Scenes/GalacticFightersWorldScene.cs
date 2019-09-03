using Guppy;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Scenes
{
    public class GalacticFightersWorldScene : Scene
    {
        #region Public Attributes
        public Group Group { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Load the default group value
            this.Group = this.provider.GetRequiredService<Peer>().Groups.GetOrCreateById(Guid.Empty);
        }
        #endregion
    }
}
