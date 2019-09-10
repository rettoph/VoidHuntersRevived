using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
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

            var part1 = this.entities.Create<ShipPart>("farseer-entity");
            var part2 = this.entities.Create<ShipPart>("farseer-entity");

            part1.Dispose();

            var part3 = this.entities.Create<ShipPart>("farseer-entity");

            part3.MaleConnectionNode.Attach(part2.FemaleConnectionNodes[0]);
        }
        #endregion
    }
}
