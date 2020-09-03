using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple helper class used to define certain NetworkEntity Action
    /// handlers based on the entities current GameAuthorization value.
    /// </summary>
    internal sealed class GameAuthorizationActions : IDisposable
    {
        #region Private Fields
        private Dictionary<GameAuthorization, Action<NetIncomingMessage>> _actions;
        private Action<NetIncomingMessage> _action;
        #endregion

        #region Public Fields
        public readonly String Type;
        public Boolean Required { get; set; }
        public Int32 SizeInBits { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="type">The name of the action beinf represented within the current contained.</param>
        /// <param name="actions">A map of which actions to preform based on a GameAuthorization index.</param>
        /// <param name="required">Whether or not the handler must be defined for all possible GameAuthorizations</param>
        internal GameAuthorizationActions(
            String type,
            Dictionary<GameAuthorization, Action<NetIncomingMessage>> actions,
            Boolean required = true,
            Int32 sizeInBits = 0)
        {
            _actions = actions;

            this.Type = type;
            this.Required = required;
            this.SizeInBits = sizeInBits;
        }

        public void Dispose()
        {
            _action = null;
            _actions.Clear();
        }
        #endregion

        #region Helper Methods
        public void DoAction(NetIncomingMessage im)
            => _action(im);

        /// <summary>
        /// Select which action to run when DoAction is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        public void ConfigureAuthorization(GameAuthorization authorization)
        {
            if (_actions.ContainsKey(authorization))
                _action = _actions[authorization];
            else if (this.Required)
                throw new InvalidOperationException($"No Action({this.Type}) defined for GameAuthorization<{authorization}>");
            else
                _action = this.DefaultOptionalAction;
        }

        private void DefaultOptionalAction(NetIncomingMessage im)
        {
            // Just skip based on message size
            im.Position += this.SizeInBits;
        }
        #endregion
    }
}
