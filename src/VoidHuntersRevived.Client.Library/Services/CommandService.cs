using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.IO;

namespace VoidHuntersRevived.Client.Library.Services
{
    public class CommandService : Service
    {
        #region Private Fields
        private Dictionary<String, Command> _commands;
        #endregion

        #region Public Properties
        public IReadOnlyDictionary<String, Command> Commands => _commands;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _commands = new Dictionary<String, Command>();
        }
        #endregion

        /// <summary>
        /// Add a unique command into the command service.
        /// </summary>
        /// <param name="name">A human readable name for this command.</param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Add(String handle, Type type, String name, Object data)
            => _commands.Add(handle, new Command(type, handle, name, data));
        /// <summary>
        /// Add a unique command into the command service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handle"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void Add<T>(String handle, String name, Object data)
            => this.Add(handle, typeof(T), name, data);

        /// <summary>
        /// Bind a specific input to a command action.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="command"></param>
        public void Bind(Keys input, Command command)
        {
            // Log the change of key input...
            command.keys.Add(input);

            // Create a new event to manage this input...
        }

        /// <summary>
        /// Bind a specific input to a command action.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="handle">The command's handle</param>
        public void Bind(Keys input, String handle)
            => this.Bind(input, _commands[handle]);
    }
}
