using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Entities.TractorBeams;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Client.Entities.Ships
{
    /// <summary>
    /// A specific version of the ship entity 
    /// controllable by the current client
    /// </summary>
    public class CurrentClientShip : Ship
    {
        public CurrentClientShip(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            base.HandleAddedToScene(sender, e);

            this.TractorBeam = this.Scene.Entities.Create<CurrentClientTractorBeam>("entity:tractor_beam:current_client");
        }
    }
}
