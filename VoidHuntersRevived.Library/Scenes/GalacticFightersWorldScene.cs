using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Scenes
{
    public class GalacticFightersWorldScene : NetworkScene
    {
        #region Protected Fields
        protected World world { get; private set; }
        #endregion

        #region Constructor
        public GalacticFightersWorldScene(World world)
        {
            this.world = world;
        }
        #endregion

        #region Lifecycle Methods

        #endregion
    }
}
