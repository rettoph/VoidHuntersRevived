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
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(ShipPart))]
    internal sealed class ClientShipPartDriver : Driver<ShipPart>
    {
        #region Private Fields;
        private Camera2D _camera;
        private ServerRender _server;
        private SpriteManager _sprite;
        private Body _serverBody;
        #endregion

        #region Constructor
        public ClientShipPartDriver(SpriteManager sprite, ClientWorldScene scene, ServerRender server, ShipPart driven) : base(driven)
        {
            _camera = scene.Camera;
            _sprite = sprite;
            _server = server;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.MaleConnectionNode.Events.TryAdd<ConnectionNode>("detached", this.HandleMaleConnectionNodeDetached);

            this.driven.Actions.TryAdd("health", this.HandleHealthAction);

            var config = (driven.Configuration.Data as ShipPartConfiguration);
            _sprite.Load(config.Texture, config.Vertices);
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            _serverBody = _server.GetBodyById(this.driven.BodyId);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var fullColor = this.driven.Controller.Color == null ? (this.driven.Root.Configuration.Data as ShipPartConfiguration).DefaultColor : this.driven.Controller.Color.Value;
            var deadColor = Color.Lerp(ColorScheme.Red, fullColor, 0.2f);

            _sprite.Draw(
                this.driven.Position,
                this.driven.Rotation,
                Color.Lerp(deadColor, fullColor, this.driven.Health / 100));
        }
        #endregion

        #region Action Handlers
        private void HandleHealthAction(object sender, NetIncomingMessage arg)
        {
            this.driven.Health = arg.ReadSingle();
        }
        #endregion

        #region Event Handlers

        private void HandleMaleConnectionNodeDetached(object sender, ConnectionNode arg)
        {
            if (this.driven.IsRoot) // Update the server render when the chain gets updated
                _serverBody.SetTransformIgnoreContacts(this.driven.Position, this.driven.Rotation);
        }
        #endregion
    }
}
