using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity
    {
        public TractorBeam(EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
        }

        public TractorBeam(Guid id, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.CollidesWith = Category.None;
            this.CreateFixture(new CircleShape(1f, 1f));
        }

        public override Body CreateBody(World world, Vector2 position = default(Vector2), float rotation = 0)
        {
            var body = base.CreateBody(world, position, rotation);

            body.IsSensor = true;

            return body;
        }
    }
}
