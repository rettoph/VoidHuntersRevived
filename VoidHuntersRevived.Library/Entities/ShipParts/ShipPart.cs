using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.MetaData;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract class ShipPart : TractorableEntity
    {
        public readonly ShipPartData ShipPartData;
        public MaleConnectionNode MaleConnectionNode { get; private set; }
        public Matrix RotationMatrix { get; private set; }

        private SpriteBatch _spriteBatch;

        public ShipPart(SpriteBatch spriteBatch, IServiceProvider provider, EntityInfo info, IGame game) : base(info, game)
        {
            _spriteBatch = spriteBatch;

            this.Visible = true;

            this.ShipPartData = info.Data as ShipPartData;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create the male connection node
            this.MaleConnectionNode = this.Scene.Entities.Create<MaleConnectionNode>("entity:connection_node:male", null, this.ShipPartData.MaleConnection, this);

            /*
            var fixture = this.Body.CreateFixture(new PolygonShape(new Vertices(new Vector2[] {
                new Vector2(-0.5f, -0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-0.5f, 0.5f)
            }), 10f));

            var shape = fixture.Shape as PolygonShape;
            shape.Clone();
            */
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.Body.BodyType = BodyType.Dynamic;
            this.Body.Restitution = 0.80f;
            this.Body.Friction = 0f;
            this.Body.LinearDamping = 1f;
            this.Body.AngularDamping = 1f;
            this.Body.CollidesWith = Category.Cat1;
            this.Body.CollisionCategories = Category.Cat2;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the current parts rotation matrix
            this.RotationMatrix = Matrix.CreateRotationZ(this.Body.Rotation);
        }
    }
}
