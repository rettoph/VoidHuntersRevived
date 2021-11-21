using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Services
{
    public class PlayerService : ServiceList<Player>
    {
        #region Private Fields
        private GuppyServiceProvider _provider;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _provider = provider;
        }

        protected override void Release()
        {
            base.Release();

            _provider = default;
        }
        #endregion


        public UserPlayer CreateUserPlayer(IUser user)
        {
            return this.Create<UserPlayer>(_provider, ServiceConfigurationKeys.Players.UserPlayer, (player, _, _) =>
            {
                player.User = user;
            });
        }
    }
}
