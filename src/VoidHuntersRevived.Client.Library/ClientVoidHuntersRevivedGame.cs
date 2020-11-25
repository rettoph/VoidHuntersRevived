using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Services;
using Guppy;
using Microsoft.Xna.Framework.Input;
using Guppy.IO.Input.Services;
using Guppy.IO.Commands.Services;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands;
using VoidHuntersRevived.Client.Library.Enums;
using System.Linq;
using System.Threading;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        #region Private Fields
        private ClientPeer _client;
        private CommandService _commands;
        private DebugService _debug;
        private Boolean _renderDebug;
        private Double[] _frameTimes;
        private Int32 _frameTimeIndex;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _commands);
            provider.Service(out _debug);

            _client = provider.GetService<ClientPeer>();
            _client.TryStart();

            var user = provider.GetService<User>();
            user.Name = "Rettoph";

            _client.TryConnect("localhost", 1337, user);

            if(Console.Read() != -1)
                new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        var input = Console.ReadLine();

                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop);

                        _commands.TryExecute(input);
                    }
                })).Start();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _frameTimes = new Double[50];

            this.Scenes.Create<GameScene>();

            _debug.Lines += this.RenderFPS;

            // Start the key service...
            _commands["toggle"]["debug"].OnExcecute += this.HandleToggleDebugCommand;
        }

        protected override void Release()
        {
            base.Release();

            _commands["toggle"]["debug"].OnExcecute -= this.HandleToggleDebugCommand;
        }
        #endregion

        #region Frame Methods
        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            if(_renderDebug)
                _debug.TryDraw(gameTime);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);

            _debug.TryUpdate(gameTime);

            _frameTimes[(_frameTimeIndex = (_frameTimeIndex + 1) % _frameTimes.Length)] = gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        #endregion

        #region Event Handlers
        private string RenderFPS(GameTime gameTime)
            => $"FPS: {(1000 / _frameTimes.Average()).ToString("#,#00.0")}\n";
        #endregion

        #region Command Handlers
        private CommandResponse HandleToggleDebugCommand(ICommand sender, CommandInput input)
        {
            if ((DebugType)input["type"] == DebugType.Data)
            {
                _renderDebug = !_renderDebug;

                return CommandResponse.Success($"Set RenderDebug to {_renderDebug}");
            }

            return CommandResponse.Empty;
        }
        #endregion
    }
}
