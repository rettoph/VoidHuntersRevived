using Guppy.MonoGame.Utilities.Cameras;
using Guppy.MonoGame;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Simulations.Components;
using MonoGame.Extended.Entities.Systems;
using Guppy.Common.Collections;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Helpers;
using Guppy.MonoGame.Primitives;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations;
using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;
using VoidHuntersRevived.Common.Entities.Tractoring;
using VoidHuntersRevived.Common.Entities.Tractoring.Services;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    [GuppyFilter<LocalGameGuppy>]
    internal sealed class DrawAimSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder PilotableAspect = Aspect.All(new[]
        {
            typeof(Pilotable),
            typeof(Predictive)
        });

        private readonly ISimulationService _simulations;
        private readonly ITractorBeamService _tractorBeams;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private PrimitiveShape _shape;

        private ComponentMapper<Pilotable> _pilotable;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Drawable> _drawable;
        private ComponentMapper<TractorBeamEmitter> _tractorBeamEmitters;
        private ComponentMapper<Tractorable> _tractorables;

        public DrawAimSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera,
            ITractorBeamService tractorBeams,
            ISimulationService simulations) : base(PilotableAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _simulations = simulations;
            _tractorBeams = tractorBeams;
            _shape = new PrimitiveShape(Vector2Helper.CreateCircle(0.25f, 16));

            _pilotable = default!;
            _bodies = default!;
            _nodes = default!;
            _drawable = default!;
            _tractorBeamEmitters = default!;
            _tractorables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotable = mapperService.GetMapper<Pilotable>();
            _bodies = mapperService.GetMapper<Body>();
            _nodes = mapperService.GetMapper<Node>();
            _drawable = mapperService.GetMapper<Drawable>();
            _tractorBeamEmitters = mapperService.GetMapper<TractorBeamEmitter>();
            _tractorables = mapperService.GetMapper<Tractorable>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach (var entityId in this.subscription.ActiveEntities)
            {
                var pilotable = _pilotable.Get(entityId);
                var transformation = pilotable.Aim.Value.ToTranslation();
                var color = Color.Yellow;

                _primitiveBatch.Trace(_shape, in color, ref transformation);

                this.DrawTractorTarget(pilotable);
            }

            _primitiveBatch.End();
        }

        private void DrawTractorTarget(Pilotable pilotable)
        {
            if (!_tractorBeamEmitters.TryGet(pilotable.EntityId, out TractorBeamEmitter? emitter))
            {
                return;
            }

            if(emitter.TractorBeam is not null)
            {
                return;
            }

            if(!_tractorBeams.Query(_simulations[SimulationType.Predictive], emitter.EntityId, out int targetId))
            {
                return;
            }
            
            if(!_nodes.TryGet(targetId, out var node))
            {
                return;
            }
            
            if(!_drawable.TryGet(targetId, out var drawable))
            {
                return;
            }
            
            Matrix transformation = drawable.LocalCenterTransformation * node.WorldTransformation;
            Color color = Color.Green;
            _primitiveBatch.Trace(_shape, in color, ref transformation);
        }
    }
}
