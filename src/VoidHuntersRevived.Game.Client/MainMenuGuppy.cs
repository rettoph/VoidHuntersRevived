﻿using Autofac;
using Guppy.Game;
using Guppy.Game.MonoGame;
using Guppy.Game.MonoGame.Providers;
using Guppy.Network;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Game.Client
{
    public class MainMenuGuppy : GameGuppy
    {
        private readonly Menu _menu;

        public MainMenuGuppy(
            IClientPeer client,
            INetScope scope,
            IMenuProvider menus)
        {
            _menu = menus.Get(Menus.Main);

            client.Groups.GetById(NetScopeIds.MainMenu).Attach(scope);
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
