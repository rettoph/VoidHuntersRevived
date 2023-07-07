using Guppy.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class BodyTreeEngine : BasicEngine, 
        IStepEngine<Step>
    {
        public string name { get; } = nameof(BodyTreeEngine);

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Body, Tree>();
            foreach (var ((bodies, trees, count), _) in this.entitiesDB.QueryEntities<Body, Tree>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    trees[i].Transformation = FixMatrix.CreateRotationZ(bodies[i].Rotation) * FixMatrix.CreateTranslation(bodies[i].Position.X, bodies[i].Position.Y, Fix64.Zero);
                }
            }
        }
    }
}
