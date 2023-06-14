using Guppy.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Game.Pieces.Components;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class BodyEngine : BasicEngine,
        IReactOnAddEx<Body>, IStepEngine<Step>
    {
        public string name { get; } = nameof(BodyEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Body> entities, ExclusiveGroupStruct groupID)
        {
            var (bodies, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                IdMap bodyId = this.Simulation.Entities.GetIdMap(ids[index], groupID);

                this.Simulation.Space.CreateBody(bodyId.VhId);
            }
        }

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<EntityVhId, Body>();
            foreach (var ((vhids, bodies, count), _) in this.entitiesDB.QueryEntities<EntityVhId, Body>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    IBody bodyInstance = this.Simulation.Space.GetBody(vhids[i].Value);
                    Body bodyComponent = bodies[i];

                    bodyComponent.Position = bodyInstance.Position;
                    bodyComponent.Rotation = bodyInstance.Rotation;
                }
            }
        }
    }
}
