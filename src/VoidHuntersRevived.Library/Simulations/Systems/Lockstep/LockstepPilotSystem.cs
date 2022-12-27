using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    internal sealed class LockstepPilotSystem : EntitySystem, ILockstepSimulationSystem, ISubscriber<PilotDirectionInput>
    {
        private PilotIdMap _pilotMap;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;

        public LockstepPilotSystem(PilotIdMap pilotIdMap) : base(Aspect.All(typeof(Piloting)))
        {
            _pilotMap = pilotIdMap;
            _pilotings = default!;
            _pilotables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotings = mapperService.GetMapper<Piloting>();
            _pilotables = mapperService.GetMapper<Pilotable>();
        }

        public void Process(in PilotDirectionInput message)
        {
            var pilotId = _pilotMap.GetEntityId(message.PilotId);

            var piloting = _pilotings.Get(pilotId);

            if (piloting.PilotableId is null)
            {
                return;
            }

            var pilotable = _pilotables.Get(piloting.PilotableId.Value);

            if (pilotable is null)
            {
                piloting.PilotableId = null;
                return;
            }

            pilotable.SetDirection(message);
        }
    }
}
