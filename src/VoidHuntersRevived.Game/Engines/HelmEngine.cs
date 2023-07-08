using Guppy.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class HelmEngine : BasicEngine,
        IEventEngine<Helm_SetDirection>, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public HelmEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public string name { get; } = nameof(HelmEngine);

        public void Process(VhId vhid, Helm_SetDirection data)
        {
            IdMap id =_entities.GetIdMap(data.ShipVhId);
            ref Helm helm = ref entitiesDB.QueryMappedEntities<Helm>(id.EGID.groupID).Entity(id.EGID.entityID);

            if (data.Value)
            {
                helm.Direction |= data.Which;
            }
            else
            {
                helm.Direction &= ~data.Which;
            }
        }

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Helm, Body>();
            foreach (var ((helms, entityIds, count), groupId) in this.entitiesDB.QueryEntities<Helm>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    IdMap helmId = _entities.GetIdMap(entityIds[i], groupId);
                    IBody body = _space.GetBody(helmId.VhId);

                    FixVector2 impulse = FixVector2.Zero;
                    Helm helm = helms[i];

                    if(helm.Direction.HasFlag(Direction.Forward))
                    {
                        impulse -= FixVector2.UnitY;
                    }
                    if (helm.Direction.HasFlag(Direction.Backward))
                    {
                        impulse += FixVector2.UnitY;
                    }
                    if (helm.Direction.HasFlag(Direction.TurnLeft))
                    {
                        impulse -= FixVector2.UnitX;
                    }
                    if (helm.Direction.HasFlag(Direction.TurnRight))
                    {
                        impulse += FixVector2.UnitX;
                    }

                    impulse *= _param.ElapsedTime;

                    body.ApplyLinearImpulse(impulse);
                }
            }
        }
    }
}
