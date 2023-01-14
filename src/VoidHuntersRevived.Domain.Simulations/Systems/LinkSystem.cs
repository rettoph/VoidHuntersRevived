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
using VoidHuntersRevived.Common.Entities.Components;
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
        ISubscriber<IEvent<CreateLink>>
    {
        private static readonly AspectBuilder LinkingAspect = Aspect.All(typeof(Linkable), typeof(Linked));

        private readonly ISimulationService _simulations;
        private ComponentMapper<Linked> _linked;
        private ComponentMapper<Linking> _linkings;

        public LinkSystem(ISimulationService simulations)
        {
            _simulations = simulations;
            _linked = default!;
            _linkings = default!;
        }

        public override void Initialize(World world)
        {
            _linked = world.ComponentMapper.GetMapper<Linked>();
            _linkings = world.ComponentMapper.GetMapper<Linking>();
        }

        public void Process(in IEvent<CreateLink> message)
        {
            if(message.Source == DataSource.External)
            {
                throw new NotImplementedException();
            }

            var parentId = message.Simulation.GetEntityId(message.Data.Parent);
            var childId = message.Simulation.GetEntityId(message.Data.Child);

            var linkings = _linkings.Get(parentId);
            var link = new Linked(
                entity: message.Simulation.GetEntity(message.Data.Child),
                joint : message.Data.ChildJointId,
                parent: new EntityJoint(
                    entity: message.Simulation.GetEntity(message.Data.Parent),
                    joint: message.Data.ParentJointId));

            if(!link.Validate())
            {
                throw new NotImplementedException();
            }

            // The child is already linked to something else
            // Should we just detach?
            if(_linked.TryGet(childId, out var oldLink))
            {
                if(link == oldLink)
                { // The link already exists
                    return;
                }

                throw new NotImplementedException();
            }

            // The parent join is already linked to something else
            // Should we just detach?
            if(linkings.Children.Any(x => x.Parent == link.Parent))
            {
                throw new NotImplementedException();
            }

            linkings.Add(link);
            _linked.Put(childId, link);

            _simulations.PublishEvent(new CleanLink(link), DataSource.Internal);
        }
    }
}
