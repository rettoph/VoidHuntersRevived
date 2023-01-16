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
using VoidHuntersRevived.Domain.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using Guppy.Common.Collections;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Helpers;
using Guppy.MonoGame.Primitives;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed class DrawAimSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder PilotableAspect = Aspect.All(new[]
        {
            typeof(Pilotable),
            typeof(Predictive)
        });

        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private PrimitiveShape _shape;

        private ComponentMapper<Pilotable> _pilotable;

        public DrawAimSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera) : base(PilotableAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _shape = new PrimitiveShape(Vector2Helper.CreateCircle(0.25f, 16));

            _pilotable = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotable = mapperService.GetMapper<Pilotable>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach (var entityId in this.subscription.ActiveEntities)
            {
                var pilotable = _pilotable.Get(entityId);
                var transformation = pilotable.Aim.Target.GetTranslation();
                var color = Color.Yellow;

                // _primitiveBatch.Trace(_shape, in color, ref transformation);

                transformation = pilotable.Aim.Value.GetTranslation();
                color = Color.Green;

                _primitiveBatch.Trace(_shape, in color, ref transformation);
            }

            _primitiveBatch.End();
        }
    }
}
