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
    [Sortable<ISubscriber<IEvent<CreateJointing>>>(int.MinValue)]
    [Sortable<ISubscriber<IEvent<DestroyJointing>>>(int.MinValue)]
    internal sealed class JointSystem : BasicSystem,
        ISubscriber<IEvent<CreateJointing>>,
        ISubscriber<IEvent<DestroyJointing>>
    {
        private readonly ISimulationService _simulations;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Jointing> _jointed;
        private ComponentMapper<Jointee> _jointees;

        public JointSystem(ISimulationService simulations)
        {
            _simulations = simulations;
            _jointables = default!;
            _jointed = default!;
            _jointees = default!;
        }

        public override void Initialize(World world)
        {
            _jointables = world.ComponentMapper.GetMapper<Jointable>();
            _jointed = world.ComponentMapper.GetMapper<Jointing>();
            _jointees = world.ComponentMapper.GetMapper<Jointee>();
        }

        public void Process(in IEvent<CreateJointing> message)
        {
            var parentId = message.Target.GetEntityId(message.Data.Parent);
            var childId = message.Target.GetEntityId(message.Data.Joint);

            var jointee = _jointees.Get(parentId);
            var jointing = new Jointing(
                joint: _jointables.Get(childId).Joints[message.Data.JointId],
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
            if(jointee.Children.Any(x => x.Parent == jointing.Parent))
            {
                throw new NotImplementedException();
            }

            jointee.Add(jointing);
            _jointed.Put(childId, jointing);

            // At this point all child joints should recursively be updated? Is there a better way?
            // The tree node has very much the same transformation but the node shouldn't need to 
            // be aware of a jointing - like in the case of the head node, right?
            this.CleanLocalTransformationRecersive(childId, jointing.LocalTransformation);
        }

        public void Process(in IEvent<DestroyJointing> message)
        {
            var jointedId = message.Target.GetEntityId(message.Data.Jointed);
            var jointed = _jointed.Get(jointedId);
            var jointings = _jointees.Get(jointed.Parent.Entity.Id);

            _jointed.Delete(jointedId);
            jointings.Remove(jointed);

            
            this.CleanLocalTransformationRecersive(jointedId, Matrix.Identity);
        }

        private void CleanLocalTransformationRecersive(int entityId, Matrix transformation)
        {
            var jointable = _jointables.Get(entityId);
            foreach(var joint in jointable.Joints)
            {
                joint.LocalTransformation = joint.Configuration.Transformation * transformation;
            }

            if (!_jointees.TryGet(entityId, out var jointee))
            {
                return;
            }

            foreach (var child in jointee.Children)
            {
                child.CleanTransformation();
                this.CleanLocalTransformationRecersive(child.Joint.Entity.Id, child.LocalTransformation);
            }
        }
    }
}
