using Guppy.Common;
using Guppy.MonoGame;
using Guppy.MonoGame.Providers;
using Guppy.Network;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Client
{
    public class MainMenuGuppy : FrameableGuppy
    {
        private readonly Menu _menu;

        public MainMenuGuppy(
            ClientPeer client,
            NetScope scope,
            IMenuProvider menus)
        {
            _menu = menus.Get(Menus.Main);

            client.Bind(scope, NetScopeIds.MainMenu);
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
