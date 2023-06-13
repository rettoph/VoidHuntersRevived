using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Systems;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Common.Components;

namespace VoidHuntersRevived.Game.Systems
{
    [AutoLoad]
    internal sealed class TreeSystem : BasicSystem, IReactiveSystem<Tree>
    {
        public void OnAdded(in Guid id, in Ref<Tree> component)
        {
            if(this.Simulation.World.Components.TryGet(component.Instance.HeadId, out Ref<Local> local))
            {

            }
        }

        public void OnRemoved(in Guid id, in Ref<Tree> component)
        {
            throw new NotImplementedException();
        }
    }
}
