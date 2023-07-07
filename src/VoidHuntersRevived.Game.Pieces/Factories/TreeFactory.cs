using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Factories;
using VoidHuntersRevived.Common.Simulations.Engines;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Game.Pieces.Factories
{
    internal sealed class TreeFactory : ITreeFactory, IEngine
    {
        private readonly IEventPublishingService _events;
        private readonly IEntitySerializationService _serialization;

        public TreeFactory(IEventPublishingService events, IEntitySerializationService serialization)
        {
            _events = events;
            _serialization = serialization;
        }

        public void Create(VhId vhid, IEntityType<TreeDescriptor> tree, IEntityType<PieceDescriptor> head)
        {
            VhId headId = vhid.Create(1);
            _events.Publish(CreateEntity.CreateEvent(
                type: tree,
                vhid: vhid,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Tree(headId));
                }));

            _events.Publish(CreateEntity.CreateEvent(
                type: head,
                vhid: headId,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init(new Node(vhid));
                }));
        }

        public void Create(VhId vhid, IEntityType<TreeDescriptor> tree, EntityData nodes)
        {
            IdMap headId = _serialization.Deserialize(vhid, nodes);

            _events.Publish(CreateEntity.CreateEvent(
                type: tree,
                vhid: vhid,
                initializer: (ref EntityInitializer initializer) =>
                {
                    initializer.Init<Tree>(new Tree(headId.VhId));
                }));
        }
    }
}
