using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists;
using Guppy.EntityComponent.Lists;
using Guppy.Network.Interfaces;
using Guppy.Network.Security;
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
        public UserPlayer CreateUserPlayer(User user)
        {
            return this.Create<UserPlayer>((player, _, _) =>
            {
                player.User = user;
            });
        }
    }
}
