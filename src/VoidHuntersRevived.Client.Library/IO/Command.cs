using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.IO
{
    public class Command
    {
        #region Internal Properties
        internal List<Keys> keys { get; private set; }
        #endregion

        #region Public Fields
        /// <summary>
        /// The command type, simple identifier used
        /// by handlers to determin what command type is recieved.
        /// 
        /// Register & bind to commands via the CommandService.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// A unique identifier for this command.
        /// </summary>
        public readonly String Handle;

        /// <summary>
        /// A human readable name for this particular command.
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// Some unique data relevant to the command.
        /// </summary>
        public readonly Object Data;
        #endregion

        #region Public Properties
        /// <summary>
        /// A list of all keys that can trigger this
        /// command.
        /// </summary>
        public IReadOnlyCollection<Keys> Keys => this.keys;

        public Command(Type type, string handle, string name, object data)
        {
            this.keys = new List<Keys>();

            this.Type = type;
            this.Handle = handle;
            this.Name = name;
            this.Data = data;
        }
        #endregion


    }
}
