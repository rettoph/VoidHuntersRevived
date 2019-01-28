using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Entities.Ships;
using VoidHuntersRevived.Client.Layers;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;

namespace VoidHuntersRevived.Client.Scenes
{
    public class ClientMainScene : MainScene, IDataHandler
    {
        public Camera Camera { get; set; }
        public Cursor Cursor { get; set; }
        private GraphicsDevice _grapihcs;

        private ClientPeer _client;
        private ClientGroup _group;

        public ClientMainScene(IPeer peer, GraphicsDevice graphics, IServiceProvider provider, IGame game) : base(provider, game)
        {
            _client = peer as ClientPeer;
            _grapihcs = graphics;

            this.Visible = true;
            this.Enabled = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _group = _client.Groups.GetById(69) as ClientGroup;
            _group.DataHandler = this;
            this.Group = _group;

            var layer = this.Layers.Create<FarseerEntityLayer>();
            this.Entities.SetDefaultLayer(layer);

            // Create the basic global entities
            this.Cursor = this.Entities.Create<Cursor>("entity:cursor");
            this.Camera = this.Entities.Create<Camera>("entity:camera");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the internal group
            _group.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            _grapihcs.Clear(Color.Black);

            base.Draw(gameTime);
        }

        #region IDataHandler Implementation
        public void HandleData(NetIncomingMessage data)
        {
            switch ((DataAction)data.ReadByte())
            {
                case DataAction.Configure:
                    this.World.Gravity = data.ReadVector2();
                    var wall = this.Entities.Create<Wall>(data.ReadString(), null, data.ReadInt64());
                    wall.Read(data);
                    break;
                case DataAction.Create:
                    break;
                case DataAction.Update:
                    break;
                case DataAction.Delete:
                    break;
            }

        }

        public void HandleUserJoined(IUser user, NetConnection connection = null)
        {
            // throw new NotImplementedException();
        }

        public void HandleUserLeft(IUser user, NetConnection connection = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
