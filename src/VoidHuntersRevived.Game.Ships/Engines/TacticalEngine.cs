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
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Engines
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
            EntityId id = _entities.GetId(data.ShipVhId);
            ref Tactical tactical = ref _entities.QueryById<Tactical>(id);

            tactical.Target = data.Value;
        }

        public void Step(in Step _param)
        {
            foreach (var ((tacticals, count), groupId) in _entities.QueryEntities<Tactical>())
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
