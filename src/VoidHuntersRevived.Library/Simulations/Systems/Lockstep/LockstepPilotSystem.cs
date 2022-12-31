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
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    internal sealed class LockstepPilotSystem : EntitySystem, ILockstepSimulationSystem, ISubscriber<PilotDirectionInput>
    {
        private SimulationEntityMapper _simulationEntityMapper;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;

        public LockstepPilotSystem(SimulationEntityMapper simulationEntityMapper) : base(Aspect.All(typeof(Piloting)))
        {
            _simulationEntityMapper = simulationEntityMapper;
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
            var pilotId = _simulationEntityMapper.GetEntityId(message.PilotId, SimulationType.Lockstep);
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
