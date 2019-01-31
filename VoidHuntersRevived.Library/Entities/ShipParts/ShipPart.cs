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
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract class ShipPart : TractorableEntity
    {
        public ShipPartData ShipPartData { get; private set; }
        public MaleConnectionNode MaleConnectionNode { get; private set; }
        public Matrix RotationMatrix { get; private set; }

        public Boolean Ghost { get; private set; }

        #region Constructors
        public ShipPart(EntityInfo info, IGame game) : base(info, game)
        {
            this.Construct(info.Data as ShipPartData);
        }
        public ShipPart(Int64 id, EntityInfo info, IGame game) : base(id, info, game)
        {
            this.Construct(info.Data as ShipPartData);
        }
        private void Construct(ShipPartData data)
        {
            this.ShipPartData = data;

            this.Enabled = true;
        }
        #endregion

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
            this.Body.Restitution = 1f;
            this.Body.Friction = 0f;
            this.Body.LinearDamping = 1f;
            this.Body.AngularDamping = 2f;

            this.SetGhost(true);
            this.UpdateRotationMatrix();
        }

        /// <summary>
        /// Ghosted shipparts are "detached freefloating" parts. The will only collide
        /// with the world boundries. Nothing else, not even themselves.
        /// </summary>
        /// <param name="ghost"></param>
        public void SetGhost(Boolean ghost)
        {
            if (ghost != this.Ghost)
            {
                if (ghost)
                {
                    this.Body.CollidesWith = Category.Cat1;
                    this.Body.CollisionCategories = Category.Cat2;
                    this.Body.SleepingAllowed = true;
                    this.Body.Mass = 0f;
                    this.Body.IsBullet = false;
                    this.SetEnabled(false);

                    this.Ghost = ghost;
                }
                else
                {
                    this.Body.CollidesWith = Category.Cat1 | Category.Cat3;
                    this.Body.CollisionCategories = Category.Cat3;
                    this.Body.SleepingAllowed = false;
                    this.Body.Mass = 10f;
                    this.Body.IsBullet = true;
                    this.SetEnabled(true);

                    this.Ghost = ghost;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.UpdateRotationMatrix();
        }

        /// <summary>
        /// Updates the current rotation matrix
        /// </summary>
        private void UpdateRotationMatrix()
        {
            this.RotationMatrix = Matrix.CreateRotationZ(this.Body.Rotation);
        }
    }
}
