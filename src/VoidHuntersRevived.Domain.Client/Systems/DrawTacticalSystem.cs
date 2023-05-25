using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Domain.Client.Systems
{
    internal sealed class DrawTacticalSystem : EntityDrawSystem
    {
        private static readonly AspectBuilder TacticalAspect = Aspect.All(new[]
        {
            typeof(Predictive),
            typeof(Tactical)
        });

        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Camera2D _camera;
        private ComponentMapper<Tactical> _tacticals = null!;
        private PrimitiveShape _shape;

        public DrawTacticalSystem(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Camera2D camera) : base(TacticalAspect)
        {
            _primitiveBatch = primitiveBatch;
            _camera = camera;
            _shape = new PrimitiveShape(new[]
            {
                Vector2.Zero,
                Vector2.UnitX,
                Vector2.One,
                Vector2.UnitY,
                Vector2.Zero,
            });
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tacticals = mapperService.GetMapper<Tactical>();
        }

        public override void Draw(GameTime gameTime)
        {
            _primitiveBatch.Begin(_camera);

            foreach(int entityId in this.ActiveEntities)
            {
                Tactical tactical = _tacticals.Get(entityId);
                Matrix transformation = Matrix.CreateTranslation((float)tactical.Value.X - 0.5f, (float)tactical.Value.Y - 0.5f, 0);

                _primitiveBatch.Trace(_shape, Color.Blue, transformation);
            }

            _primitiveBatch.End();
        }
    }
}
