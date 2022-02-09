using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using Microsoft.Xna.Framework;
using Guppy.Network;
using Guppy.Network.Security.Structs;
using Guppy.Network.Security.Enums;
using Guppy.Network.Security.Lists;
using Guppy.Network.Security.EventArgs;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Network.Security;

namespace VoidHuntersRevived.Server
{
    public class ServerPrimaryGame : PrimaryGame
    {
        #region Public Properties
        public ServerPeer Server { get; private set; }
        public override Peer Peer => this.Server;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Server = provider.GetService<ServerPeer>();

            this.Server.Users.OnEvent += this.HandleServerUserListEvent;
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.Scenes.Scene.TryStartAsync();
            this.Server.TryStart(1337, new[] { new Claim("username", "Server", ClaimType.Public) });
        }

        protected override void PreUninitialize()
        {
            base.PreUninitialize();

            this.Server.TryStop();
            this.Scenes.Scene.TryStopAsync();
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Server.Users.OnEvent -= this.HandleServerUserListEvent;
        }
        #endregion

        #region Event Handlers
        private void HandleServerUserListEvent(UserList sender, UserListEventArgs args)
        {
            switch(args.Action)
            {
                case UserListAction.Added:
                    this.HandleUserAdded(args.User);
                    break;
            }
        }

        private void HandleUserAdded(User user)
        {
            if(this.Scenes.Scene is PrimaryScene scene)
            {
                scene.Room.Users.TryAdd(user);
            }
        }
        #endregion
    }
}
