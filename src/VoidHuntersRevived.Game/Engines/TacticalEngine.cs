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
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Enums;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TacticalEngine : BasicEngine,
        IStepEngine<Step>
    {
        private static readonly Fix64 AimDamping = Fix64.One / (Fix64)32;

        public string name { get; } = nameof(TacticalEngine);

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Tactical>();
            foreach (var ((tacticals, count), groupId) in this.entitiesDB.QueryEntities<Tactical>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    Tactical tactical = tacticals[i];

                    Fix64 amount = Fix64.Min((Fix64)_param.ElapsedTime / AimDamping, Fix64.One);
                    tactical.Value = FixVector2.Lerp(
                        v1: tactical.Value,
                        v2: tactical.Target,
                        amount: amount);
                }
            }
        }
    }
}
