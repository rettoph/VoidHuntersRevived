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
        protected override void Initialize()
        {
            base.Initialize();

            this.entities.Create<FarseerEntity>("farseer-entity");
        }
    }
}
