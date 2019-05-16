using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class ShipPart : DrivenEntity
    {
        public Body Body { get; private set; }

        public ShipPart(
            World world, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger,
            IServiceProvider provider) : 
                base(
                    configuration,
                    scene,
                    logger,
                    provider,
                    "driver:ship-part")
        {
            this.Body = BodyFactory.CreateBody(
                world: world,
                userData: this,
                bodyType: BodyType.Dynamic);
        }

        public ShipPart(
            Guid id, 
            World world, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger, 
            IServiceProvider provider) : 
                base(
                    id, 
                    configuration, 
                    scene, 
                    logger,
                    provider,
                    "driver:ship-part")
        {
        }

        protected override void Boot()
        {
            base.Boot();

            FixtureFactory.AttachPolygon(new Vertices(new Vector2[] {
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, -0.5f)
            }), 1f, this.Body, this);
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
