using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.MetaData;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : ShipPart
    {
        public Hull(EntityInfo info, IGame game) : base(info, game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            base.HandleAddedToScene(sender, e);

            this.Body.BodyType = BodyType.Dynamic;
            this.Body.CreateFixture(new PolygonShape((this.Info.Data as HullData).Vertices, 10f));

            this.Body.Restitution = 0.80f;
            this.Body.Friction = 0f;
            this.Body.LinearDamping = 1f;
            this.Body.AngularDamping = 1f;
            this.Body.CollidesWith = Category.Cat1;
            this.Body.CollisionCategories = Category.Cat2;
        }
    }
}
