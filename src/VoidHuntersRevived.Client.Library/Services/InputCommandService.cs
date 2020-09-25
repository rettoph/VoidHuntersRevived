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
        #region Private Structs
        private struct KeyValue
        {
            public Keys Key;
            public Boolean Value;
        }
        #endregion

        #region Private Fields
        private CommandService _commandService;
        private KeyService _keyService;
        private Dictionary<Keys, String> _commands;
        private Dictionary<KeyValue, Command> _cache;
        private Command _command;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _commandService);
            provider.Service(out _keyService);

            _commands = new Dictionary<Keys, String>();
            _commands.Add(Keys.A, "set direction -d=left -v={0}");

            _cache = new Dictionary<KeyValue, Command>();
            _keyService[Keys.A].OnKeyPressed += this.HandleKeyChanged;
            _keyService[Keys.A].OnKeyReleased += this.HandleKeyChanged;
        }

        private void HandleKeyChanged(KeyService.KeyManager key)
        {
            var kv = new KeyValue()
            {
                Key = key.Key,
                Value = key.Pressed
            };

            try
            {
                _command = _cache[kv];
            }
            catch(KeyNotFoundException e)
            {
                _command = _cache[kv] = _commandService.TryBuild(String.Format(_commands[kv.Key], kv.Value ? "true" : "false"));
            }

            _commandService.TryExecute(_command);
        }
        #endregion
    }
}
