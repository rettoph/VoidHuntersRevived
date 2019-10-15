using FarseerPhysics;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(ShipPart))]
    internal sealed class ClientShipPartDriver : Driver<ShipPart>
    {
        #region Private Fields
        private ContentLoader _content;
        private Texture2D _texture;
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;
        private Vector3 _position;
        private Vector2 _origin;
        private Boolean _textureLoaded;
        private ServerRender _server;
        #endregion

        #region Constructor
        public ClientShipPartDriver(SpriteBatch spriteBatch, ClientWorldScene scene, ContentLoader content, ServerRender server, ShipPart driven) : base(driven)
        {
            _spriteBatch = spriteBatch;
            _camera = scene.Camera;
            _content = content;
            _server = server;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<ShipPart.ChainUpdate>("chain:updated", this.HandleChainUpdated);
            this.driven.MaleConnectionNode.Events.TryAdd<ConnectionNode>("detached", this.HandleMaleConnectionNodeDetached);

            this.driven.Actions.TryAdd("health", this.HandleHealthAction);

            var config = (driven.Configuration.Data as ShipPartConfiguration);
            _texture = _content.TryGet<Texture2D>(config.Texture);

            if(_texture == default(Texture2D))
            {
                _textureLoaded = false;
            }
            else
            {
                _textureLoaded = true;
                _origin = Vector2.Zero;
                var verticies = config.Vertices.SelectMany(v => v);
                Vector2 min = ConvertUnits.ToDisplayUnits(new Vector2(verticies.Min(v => v.X), verticies.Min(v => v.Y)));
                Vector2 max = ConvertUnits.ToDisplayUnits(new Vector2(verticies.Max(v => v.X), verticies.Max(v => v.Y)));
                Vector2 center = (min + max) / 2;

                _origin = (new Vector2(_texture.Width, _texture.Height)/2) - center;
            }
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Update the internal Vector3 data
            this.driven.Position.Deconstruct(out _position.X, out _position.Y);

            if (_textureLoaded && _camera.Frustum.Contains(_position).HasFlag(ContainmentType.Contains))
            {
                var fullColor = Color.Lerp(this.driven.Root.IsBridge ? Color.Blue : Color.Orange, Color.White, 0.1f);
                var deadColor = Color.Lerp(Color.DarkRed, fullColor, 0.2f);

                _spriteBatch.Draw(
                    texture: _texture,
                    position: this.driven.Position,
                    sourceRectangle: _texture.Bounds,
                    color: Color.Lerp(deadColor, fullColor, this.driven.Health / 100),
                    rotation: this.driven.Rotation,
                    origin: _origin,
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
        #endregion

        #region Action Handlers
        private void HandleHealthAction(object sender, NetIncomingMessage arg)
        {
            this.driven.Health = arg.ReadSingle();
        }
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ShipPart.ChainUpdate arg)
        {
            if(this.driven.IsRoot) // Update the server render when the chain gets updated
                _server.GetBodyById(this.driven.BodyId).SetTransformIgnoreContacts(this.driven.Position, this.driven.Rotation);
        }

        private void HandleMaleConnectionNodeDetached(object sender, ConnectionNode arg)
        {
            if (this.driven.IsRoot) // Update the server render when the chain gets updated
                _server.GetBodyById(this.driven.BodyId).SetTransformIgnoreContacts(this.driven.Position, this.driven.Rotation);
        }
        #endregion
    }
}
