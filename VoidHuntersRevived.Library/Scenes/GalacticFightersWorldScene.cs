using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Players;
using Guppy;
using Guppy.Collections;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Scenes
{
    public abstract class GalacticFightersWorldScene : NetworkScene
    {
        #region Protected Fields
        protected World world { get; private set; }

        protected OrderableCollection<Player> players { get; private set; }
        #endregion

        #region Constructor
        public GalacticFightersWorldScene(World world, OrderableCollection<Player> players)
        {
            this.world = world;
            this.players = players;
        }
        #endregion

        #region Lifecycle Methods

        #endregion
    }
}
