using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TacticalEngine : BasicEngine,
        IEventEngine<Tactical_SetTarget>,
        IStepEngine<Step>
    {
        private static readonly Fix64 AimDamping = Fix64.One / (Fix64)32;

        private readonly IEntityService _entities;

        public TacticalEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public string name { get; } = nameof(TacticalEngine);

        public void Process(VhId eventId, Tactical_SetTarget data)
        {
            IdMap id = _entities.GetIdMap(data.ShipVhId);
            ref Tactical tactical = ref entitiesDB.QueryMappedEntities<Tactical>(id.EGID.groupID).Entity(id.EGID.entityID);

            tactical.Target = data.Value;
        }

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Tactical>();
            foreach (var ((tacticals, count), groupId) in this.entitiesDB.QueryEntities<Tactical>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    ref Tactical tactical = ref tacticals[i];

                    Fix64 amount = Fix64.Min(_param.ElapsedTime / AimDamping, Fix64.One);
                    tactical.Value = FixVector2.Lerp(
                        v1: tactical.Value,
                        v2: tactical.Target,
                        amount: amount);
                }
            }
        }
    }
}
