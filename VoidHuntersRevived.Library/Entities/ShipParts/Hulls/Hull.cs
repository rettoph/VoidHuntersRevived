using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.MetaData;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : ShipPart
    {
        public Hull(SpriteBatch spriteBatch, IServiceProvider provider, EntityInfo info, IGame game) : base(spriteBatch, provider, info, game)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Body.CreateFixture(new PolygonShape((this.Info.Data as HullData).Vertices, 0.1f));
        }
    }
}
