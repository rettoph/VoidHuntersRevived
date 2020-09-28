using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Services;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Services
{
    /// <summary>
    /// Simple service to automate the invocation 
    /// of a command based an input service.
    /// </summary>
    public class InputCommandService : Service
    {
        #region Private Fields
        private CommandService _commandService;
        private ButtonService _keyService;
        private Dictionary<Keys, String> _commands;
        private Dictionary<ButtonService.ButtonValue, CommandArguments> _cache;
        private CommandArguments _command;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _commandService);
            provider.Service(out _keyService);

            _commands = new Dictionary<Keys, String>();
            _commands.Add(Keys.A, "set direction -d=left -v={0}");

            _cache = new Dictionary<ButtonService.ButtonValue, CommandArguments>();
            _keyService[Keys.A].OnKeyPressed += this.HandleButtonChanged;
            _keyService[Keys.A].OnKeyReleased += this.HandleButtonChanged;
        }

        private void HandleButtonChanged(ButtonService.ButtonManager buttonManager, ButtonService.ButtonValue args)
        {
            try
            {
                _command = _cache[args];
            }
            catch(KeyNotFoundException e)
            {
                _command = _cache[args] = _commandService.TryBuild(String.Format(_commands[args.Which.KeyboardKey], args.State == ButtonState.Pressed ? "true" : "false"));
            }

            _commandService.TryExecute(_command);
        }
        #endregion
    }
}
