using Guppy.Attributes;
using Guppy.Common;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Domain.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class LinkSystem : BasicSystem,
        ISubscriber<IEvent<CreateJointing>>
    {
        private readonly ISimulationService _simulations;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Jointed> _jointed;
        private ComponentMapper<Jointings> _jointings;

        public LinkSystem(ISimulationService simulations)
        {
            _simulations = simulations;
            _jointables = default!;
            _jointed = default!;
            _jointings = default!;
        }

        public override void Initialize(World world)
        {
            _jointables = world.ComponentMapper.GetMapper<Jointable>();
            _jointed = world.ComponentMapper.GetMapper<Jointed>();
            _jointings = world.ComponentMapper.GetMapper<Jointings>();
        }

        public void Process(in IEvent<CreateJointing> message)
        {
            if(message.Source == DataSource.External)
            {
                throw new NotImplementedException();
            }

            var parentId = message.Simulation.GetEntityId(message.Data.Parent);
            var childId = message.Simulation.GetEntityId(message.Data.Child);

            var jointings = _jointings.Get(parentId);
            var jointed = new Jointed(
                joint: _jointables.Get(childId).Joints[message.Data.ChildJointId],
                parent: _jointables.Get(parentId).Joints[message.Data.ParentJointId]);

            if(!jointed.Validate())
            {
                throw new NotImplementedException();
            }

            // The child is already linked to something else
            // Should we just detach?
            if(_jointed.TryGet(childId, out var oldLink))
            {
                if(jointed == oldLink)
                { // The link already exists
                    return;
                }

                throw new NotImplementedException();
            }

            // The parent join is already linked to something else
            // Should we just detach?
            if(jointings.Children.Any(x => x.Parent == jointed.Parent))
            {
                throw new NotImplementedException();
            }

            jointings.Add(jointed);
            _jointed.Put(childId, jointed);

            _simulations.PublishEvent(new CleanJointing(jointed), DataSource.Internal);
        }
    }
}
