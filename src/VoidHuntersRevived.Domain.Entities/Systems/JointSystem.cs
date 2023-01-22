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
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class JointSystem : BasicSystem,
        ISubscriber<IEvent<CreateJointing>>
    {
        private readonly ISimulationService _simulations;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Jointing> _jointed;
        private ComponentMapper<Jointings> _jointings;

        public JointSystem(ISimulationService simulations)
        {
            _simulations = simulations;
            _jointables = default!;
            _jointed = default!;
            _jointings = default!;
        }

        public override void Initialize(World world)
        {
            _jointables = world.ComponentMapper.GetMapper<Jointable>();
            _jointed = world.ComponentMapper.GetMapper<Jointing>();
            _jointings = world.ComponentMapper.GetMapper<Jointings>();
        }

        public void Process(in IEvent<CreateJointing> message)
        {
            var parentId = message.Simulation.GetEntityId(message.Data.Parent);
            var childId = message.Simulation.GetEntityId(message.Data.Joint);

            var jointings = _jointings.Get(parentId);
            var jointing = new Jointing(
                joint: _jointables.Get(childId).Joints[message.Data.ChildJointId],
                parent: _jointables.Get(parentId).Joints[message.Data.ParentJointId]);

            if(!jointing.Validate())
            {
                throw new NotImplementedException();
            }

            // The child is already linked to something else
            // Should we just detach?
            if(_jointed.TryGet(childId, out var oldLink))
            {
                if(jointing == oldLink)
                { // The link already exists
                    return;
                }

                throw new NotImplementedException();
            }

            // The parent join is already linked to something else
            // Should we just detach?
            if(jointings.Children.Any(x => x.Parent == jointing.Parent))
            {
                throw new NotImplementedException();
            }

            jointings.Add(jointing);
            _jointed.Put(childId, jointing);

            // Update local transformations
            var transformation = jointing.LocalTransformation;

            var jointable = _jointables.Get(jointing.Joint.Entity);
            foreach (var joint in jointable.Joints)
            {
                joint.LocalTransformation = joint.Configuration.Transformation * transformation;
            }

            message.Simulation.PublishEvent(new CleanJointed(jointing));
        }
    }
}
