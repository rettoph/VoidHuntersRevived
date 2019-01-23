using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.MetaData;
using System.Linq;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : ShipPart
    {
        public readonly HullData HullData;
        private SpriteBatch _spriteBatch;

        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }

        public Hull(SpriteBatch spriteBatch, IServiceProvider provider, EntityInfo info, IGame game) : base(spriteBatch, provider, info, game)
        {
            _spriteBatch = spriteBatch;

            this.HullData = this.Info.Data as HullData;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Body.CreateFixture(new PolygonShape(this.HullData.Vertices, 0.1f));

            // Create the female connection nodes
            this.FemaleConnectionNodes = this.HullData.FemaleConnections
                .Select(fcnd =>
                {
                    return this.Scene.Entities.Create<FemaleConnectionNode>("entity:connection_node:female", null, fcnd, this);
                })
                .ToArray();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
