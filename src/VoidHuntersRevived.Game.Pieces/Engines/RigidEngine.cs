using Guppy.Attributes;
using Guppy.Resources.Providers;
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
using VoidHuntersRevived.Game.Pieces.Resources;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class RigidEngine : BasicEngine,
        IEventEngine<AddedNode>
    {
        private readonly IResourceProvider _resources;

        public RigidEngine(IResourceProvider resources)
        {
            _resources = resources;
        }

        public void Process(VhId id, AddedNode data)
        {
            IdMap nodeId = this.Simulation.Entities.GetIdMap(data.NodeId);
            IdMap treeId = this.Simulation.Entities.GetIdMap(data.TreeId);

            // Node is not a rigid entity
            if(!this.entitiesDB.TryGetEntity<ResourceId<Rigid>>(nodeId.EGID, out ResourceId<Rigid> rigidId))
            {
                return;
            }

            Rigid rigid = _resources.Get(rigidId)!;
            IBody body = this.Simulation.Space.GetOrCreateBody(treeId.VhId);
            body.Create(rigid.Shapes[0], nodeId.VhId);
        }
    }
}
