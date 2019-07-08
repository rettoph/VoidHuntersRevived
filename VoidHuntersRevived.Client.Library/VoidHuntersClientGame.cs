using System;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library
{
    public class VoidHuntersClientGame : VoidHuntersGame
    {
        protected ClientPeer client;

        public VoidHuntersClientGame(ClientPeer client, IServiceProvider provider) : base(provider)
        {
            this.client = client;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.SetScene(this.CreateScene<LobbyScene>());

            this.client.OnStatusChanged += this.HandleStatusChanged;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            var user = new User();
            user.Set("name", "Tony");
            this.client.Connect("localhost", 1337, user);
        }

        protected override void update(GameTime gameTime)
        {
            this.client.Update();

            base.update(gameTime);
        }

        private void HandleStatusChanged(object sender, NetIncomingMessage e)
        {
            switch ((NetConnectionStatus)e.ReadByte())
            {
                case NetConnectionStatus.None:
                    break;
                case NetConnectionStatus.InitiatedConnect:
                    break;
                case NetConnectionStatus.ReceivedInitiation:
                    break;
                case NetConnectionStatus.RespondedAwaitingApproval:
                    break;
                case NetConnectionStatus.RespondedConnect:
                    break;
                case NetConnectionStatus.Connected:
                    this.SetScene(this.CreateScene<VoidHuntersClientWorldScene>());
                    break;
                case NetConnectionStatus.Disconnecting:
                    break;
                case NetConnectionStatus.Disconnected:
                    this.SetScene(this.CreateScene<LobbyScene>());
                    break;
            }
        }
    }
}