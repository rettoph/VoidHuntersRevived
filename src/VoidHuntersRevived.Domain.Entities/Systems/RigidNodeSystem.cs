using Guppy.Attributes;
using Guppy.Common;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class RigidNodeSystem : EntitySystem,
        ISubscriber<Added<Node, Tree>>,
        ISubscriber<Removed<Node, Tree>>
    {
        private ComponentMapper<Rigid> _rigids;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Body> _bodies;
        private Queue<Fixture> _buffer;
        private Dictionary<int, Fixture[]> _fixtures;

        public RigidNodeSystem() : base(Aspect.All(typeof(Rigid), typeof(Node)))
        {
            _buffer = new Queue<Fixture>();
            _fixtures = new Dictionary<int, Fixture[]>();

            _rigids = default!;
            _nodes = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
            _nodes = mapperService.GetMapper<Node>();
            _bodies = mapperService.GetMapper<Body>();
        }

        private void AddRigid(int entityId)
        {
            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            var node = _nodes.Get(entityId);
            var rigid = _rigids.Get(entityId);
            var body = _bodies.Get(node.Tree?.EntityId ?? throw new NotImplementedException());

            var transformation = node.LocalTransformation;
            var fixtures = new Fixture[rigid.Shapes.Length];

            for(var i=0; i<rigid.Shapes.Length; i++)
            {
                fixtures[i] = new Fixture(rigid.Shapes[i].Clone(ref transformation));
                fixtures[i].Tag = entityId;

                body.Add(fixtures[i]);
            }

            _fixtures.Add(entityId, fixtures);
        }

        private void RemoveRigid(int entityId)
        {
            if(!_fixtures.ContainsKey(entityId))
            {
                return;
            }

            foreach(var fixture in _fixtures[entityId])
            {
                fixture.Body.Remove(fixture);
            }

            _fixtures.Remove(entityId);
        }

        private void UpdateRigid(int entityId)
        {
            bool interested = this.subscription.IsInterested(entityId);
            bool added = _fixtures.ContainsKey(entityId);

            if (interested && !added)
            {
                this.AddRigid(entityId);
                return;
            }

            if (!interested && added)
            {
                this.RemoveRigid(entityId);
                return;
            }
        }

        public void Process(in Added<Node, Tree> message)
        {
            this.AddRigid(message.Item.EntityId);
        }

        public void Process(in Removed<Node, Tree> message)
        {
            this.RemoveRigid(message.Item.EntityId);
        }
    }
}
