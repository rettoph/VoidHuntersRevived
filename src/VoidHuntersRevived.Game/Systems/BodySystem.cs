using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Systems;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Common.Components;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Systems
{
    [AutoLoad]
    internal sealed class BodySystem : BasicSystem,
        IReactiveSystem<Body>,
        IReactiveSystem<PiecePropertyId<Rigid>>,
        IStepSystem<Body>
    {
        public void OnAdded(in Guid id, in Ref<Body> body)
        {
            IBody spaceBody = this.Simulation.Space.CreateBody(id);
            spaceBody.SetTransform(body.Instance.Position, body.Instance.Rotation);
        }

        public void Step(Step step, in Guid id, ref Body body)
        {
            IBody spaceBody = this.Simulation.Space.GetBody(id);

            body.Position = spaceBody.Position;
            body.Rotation = spaceBody.Rotation;
        }

        public void OnRemoved(in Guid id, in Ref<Body> body)
        {
            this.Simulation.Space.RemoveBody(id);
        }

        public void OnAdded(in Guid id, in Ref<PiecePropertyId<Rigid>> component)
        {
            throw new NotImplementedException();
        }

        public void OnRemoved(in Guid id, in Ref<PiecePropertyId<Rigid>> component)
        {
            throw new NotImplementedException();
        }
    }
}
