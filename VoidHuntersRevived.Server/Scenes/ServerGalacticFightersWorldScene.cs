using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Scenes;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server.Scenes
{
    internal sealed class ServerGalacticFightersWorldScene : GalacticFightersWorldScene
    {
        #region Constructor
        public ServerGalacticFightersWorldScene(World world) : base(world)
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.entities.Create<FarseerEntity>("farseer-entity");
        }
        #endregion
    }
}
