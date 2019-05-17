using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Guppy.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Drivers;
using Lidgren.Network;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class ShipPart : NetworkEntity
    {
        private IServiceProvider _provider;
        private ShipPartDriver _driver;

        public Body Body { get; private set; }

        public ShipPart(
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger,
            IServiceProvider provider) : 
                base(
                    configuration,
                    scene,
                    logger)
        {
            _provider = provider;
        }

        public ShipPart(
            Guid id, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger, 
            IServiceProvider provider) : 
                base(
                    id, 
                    configuration, 
                    scene, 
                    logger)
        {
            _provider = provider;
        }

        protected override void Boot()
        {
            base.Boot();

            _driver = _provider.GetService<EntityCollection>().Create<ShipPartDriver>("driver:ship-part", this);

            this.Body = BodyFactory.CreateBody(
                world: _provider.GetService<World>(),
                userData: this,
                bodyType: BodyType.Dynamic);
            this.Body.AngularDamping = 1f;
            this.Body.LinearDamping = 1f;

            FixtureFactory.AttachPolygon(new Vertices(new Vector2[] {
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, -0.5f)
            }), 1f, this.Body, this);
        }

        public override void Draw(GameTime gameTime)
        {
            _driver.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            _driver.Update(gameTime);
        }

        public override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.Body.Position = im.ReadVector2();
            this.Body.Rotation = im.ReadSingle();
            this.Body.LinearVelocity = im.ReadVector2();
            this.Body.AngularVelocity = im.ReadSingle();
        }

        public override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            om.Write(this.Body.Position);
            om.Write(this.Body.Rotation);
            om.Write(this.Body.LinearVelocity);
            om.Write(this.Body.AngularVelocity);
        }
    }
}
