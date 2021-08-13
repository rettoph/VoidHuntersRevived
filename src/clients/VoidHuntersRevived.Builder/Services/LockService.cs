using Guppy;
using Guppy.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.Enums;

namespace VoidHuntersRevived.Builder.Services
{
    public class LockService : Service
    {
        #region Private Fields
        private Dictionary<LockType, Boolean> _values;
        private CommandService _commands;
        #endregion

        #region Public Properties
        public Boolean this[LockType type] => _values[type];
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _commands);
            _values = DictionaryHelper.BuildEnumDictionary<LockType, Boolean>(fallback: true);

            _commands["lock"].OnExcecute += this.HandleLockCommand;
        }

        protected override void Release()
        {
            base.Release();

            _commands["lock"].OnExcecute -= this.HandleLockCommand;

            _values = null;
            _commands = null;
        }
        #endregion

        #region Event Handlers
        private CommandResponse HandleLockCommand(ICommand sender, CommandInput input)
        {
            var type = input.GetIfContains<LockType>("type");
            var value = input.GetIfContains<Boolean>("value");

            _values[type] = value;

            return CommandResponse.Success($"Set {type} lock state to {value}.");
        }
        #endregion
    }
}
