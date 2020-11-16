using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Threading;
using Guppy.IO.Commands.Services;

namespace VoidHuntersRevived.Server
{
    public class ServerVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        private ServerPeer _server;
        private CommandService _commands;

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _server = provider.GetService<ServerPeer>();
            _server.TryStart();

            _server.Users.OnAdded += this.HandleUserJoined;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _commands);

            this.Scenes.Create<GameScene>();
        }
        #endregion

        #region Frame Methods
        protected override void Start(bool draw, int period)
        {
            base.Start(draw, period);

            new Thread(new ThreadStart(() =>
            {
                while(this.Running)
                {
                    var input = Console.ReadLine();

                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);

                    _commands.TryExecute(input);
                }
            })).Start();
        }
        #endregion

        #region Event Handlers
        private void HandleUserJoined(object sender, User e)
        {
            var group = _server.Groups.GetOrCreateById(Guid.Empty);
            group.Users.TryAdd(e);
        }
        #endregion
    }
}
