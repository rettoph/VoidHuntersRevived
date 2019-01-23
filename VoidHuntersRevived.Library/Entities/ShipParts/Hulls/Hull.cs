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
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.MetaData;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : ShipPart
    {
        protected readonly HullData HullData;
        private Texture2D _femaleConnectionTexture;
        private SpriteBatch _spriteBatch;

        public Hull(SpriteBatch spriteBatch, IServiceProvider provider, EntityInfo info, IGame game) : base(spriteBatch, provider, info, game)
        {
            var contentLoader = provider.GetLoader<ContentLoader>();
            _femaleConnectionTexture = contentLoader.Get<Texture2D>("texture:female_connection");

            _spriteBatch = spriteBatch;

            this.HullData = this.Info.Data as HullData;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Body.CreateFixture(new PolygonShape(this.HullData.Vertices, 0.1f));
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach(FemaleConnection femaleConnection in this.HullData.FemaleConnections)
            {
                var femaleJointOffset = Vector2.Transform(femaleConnection.LocalPoint, _rotationMatrix);
                _spriteBatch.Draw(
                    texture: _femaleConnectionTexture,
                    position: this.Body.Position + femaleJointOffset,
                    sourceRectangle: _femaleConnectionTexture.Bounds,
                    color: Color.White,
                    rotation: this.Body.Rotation,
                    origin: Vector2.Zero,
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
    }
}
