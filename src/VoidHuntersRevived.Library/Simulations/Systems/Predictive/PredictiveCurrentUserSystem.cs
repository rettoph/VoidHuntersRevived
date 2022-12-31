using Guppy.Common;
using Guppy.Network.Peers;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Simulations.Systems.Predictive
{
    internal sealed class PredictiveCurrentUserSystem : EntitySystem, IPredictiveSimulationSystem,
        ISubscriber<DirectionInput>
    {
        private readonly Peer _peer;
        private readonly ISimulationService _simulations;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Pilotable> _pilotables;

        public PredictiveCurrentUserSystem(
            Peer peer, 
            ISimulationService simulations) : base(Aspect.All(typeof(AetherBody), typeof(Pilotable)).Exclude(typeof(Lockstepped)))
        {
            _peer = peer;
            _simulations = simulations;
            _pilotings = default!;
            _pilotables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotings = mapperService.GetMapper<Piloting>();
            _pilotables = mapperService.GetMapper<Pilotable>();
        }

        /// <summary>
        /// "predict" the server will update the current user's
        /// direction.
        /// </summary>
        /// <param name="message"></param>
        public void Process(in DirectionInput message)
        {
            if(_peer.Users.Current is null)
            {
                return;
            }

            if(!_simulations.UserIdMap.TryGet(_peer.Users.Current.Id, out var pilotId))
            {
                return;
            }

            var pilotEntityId = _simulations[SimulationType.Predictive].GetEntityId(pilotId);
            var piloting = _pilotings.Get(pilotEntityId);

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
