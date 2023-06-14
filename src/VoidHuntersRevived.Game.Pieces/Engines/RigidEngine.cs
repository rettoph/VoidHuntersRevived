using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class RigidEngine : BasicEngine,
        IEventEngine<AddedNode>
    {
        public void Process(VhId id, AddedNode data)
        {
            IdMap nodeId = this.Simulation.Entities.GetIdMap(data.NodeId);
            IdMap treeId = this.Simulation.Entities.GetIdMap(data.TreeId);

            // Node is not a rigid entity
            if (!this.Simulation.Entities.TryGetProperty(nodeId.EGID, out Rigid rigid))
            {
                return;
            }

            IBody body = this.Simulation.Space.GetBody(treeId.VhId);
            body.Create(rigid.Polygons[0], nodeId.VhId);
        }
    }
}
