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
using FarseerPhysics.Collision.Shapes;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class ShipPart : FarseerEntity
    {
        private IServiceProvider _provider;
        private ShipPartDriver _driver;

        public Shape Shape { get; private set; }

        #region Constructor Methods
        public ShipPart(
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger,
            IServiceProvider provider) : 
                base(
                    configuration,
                    scene,
                    logger,
                    provider)
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
                    logger,
                    provider)
        {
            _provider = provider;
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.Shape = this.CreateShape();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _driver = _provider.GetService<EntityCollection>().Create<ShipPartDriver>("driver:ship-part", this);

            // Attatch the default shape to the current ship part
            this.CreateFixture(this.Shape);
        }

        /// <summary>
        /// Create the main shape used within the current ship part.
        /// </summary>
        /// <returns></returns>
        protected virtual Shape CreateShape()
        {
            return new PolygonShape(new Vertices(new Vector2[]
            {
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(-0.5f, -0.5f)
            }), 1f);
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _driver.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _driver.Update(gameTime);
        }
        #endregion

        #region Network Methods
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
        #endregion
    }
}
