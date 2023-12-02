using Autofac;
using Guppy.Common;
using Guppy.Game;
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
    public class MainMenuGuppy : GameGuppy
    {
        private readonly Menu _menu;

        public MainMenuGuppy(
            ClientPeer client,
            NetScope netScope,
            IMenuProvider menus)
        {
            _menu = menus.Get(Menus.Main);

            client.Bind(netScope, NetScopeIds.MainMenu);
        }

        public override void Initialize(ILifetimeScope scope)
        {
            base.Initialize(scope);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
