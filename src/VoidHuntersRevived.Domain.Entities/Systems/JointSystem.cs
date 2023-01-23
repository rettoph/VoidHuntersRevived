using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Attributes;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Sortable<ISubscriber<IEvent<CleanJointed>>>(int.MinValue)]
    internal sealed class JointSystem : BasicSystem,
        ISubscriber<IEvent<CreateJointing>>,
        ISubscriber<IEvent<DestroyJointing>>,
        ISubscriber<IEvent<CleanJointed>>
    {
        private readonly ISimulationService _simulations;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Jointed> _jointed;
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
            _jointed = world.ComponentMapper.GetMapper<Jointed>();
            _jointings = world.ComponentMapper.GetMapper<Jointings>();
        }

        public void Process(in IEvent<CreateJointing> message)
        {
            var parentId = message.Simulation.GetEntityId(message.Data.Parent);
            var childId = message.Simulation.GetEntityId(message.Data.Joint);

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

            message.Simulation.PublishEvent(new CleanJointed(jointed, CleanJointed.Statuses.Create));
        }

        public void Process(in IEvent<DestroyJointing> message)
        {
            var jointedId = message.Simulation.GetEntityId(message.Data.Jointed);
            var jointed = _jointed.Get(jointedId);
            var jointings = _jointings.Get(jointed.Parent.Entity.Id);

            _jointed.Delete(jointedId);
            jointings.Remove(jointed);

            message.Simulation.PublishEvent(new CleanJointed(jointed, CleanJointed.Statuses.Destroy));
        }

        public void Process(in IEvent<CleanJointed> message)
        {
            message.Data.Jointed.Clean();
            var transformation = message.Data.Status switch
            {
                CleanJointed.Statuses.Destroy => Matrix.Identity,
                _ => message.Data.Jointed.LocalTransformation
            };

            var jointable = _jointables.Get(message.Data.Jointed.Joint.Entity);
            foreach (var joint in jointable.Joints)
            {
                joint.LocalTransformation = joint.Configuration.Transformation * transformation;
            }

            if(!_jointings.TryGet(message.Data.Jointed.Joint.Entity, out var jointings))
            {
                return;
            }

            foreach(var child in jointings.Children)
            {
                var clean = new CleanJointed(child, CleanJointed.Statuses.Clean);
                message.Simulation.PublishEvent(clean);
            }
        }
    }
}
