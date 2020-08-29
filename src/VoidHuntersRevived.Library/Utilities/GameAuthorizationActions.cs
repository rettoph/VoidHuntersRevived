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
        #endregion

        #region Constructor
        internal GameAuthorizationActions(
            String type,
            GameAuthorization authorization,
            Dictionary<GameAuthorization, Action<NetIncomingMessage>> actions,
            Boolean required)
        {
            _actions = actions;

            this.Type = type;
            this.Required = required;

            this.ConfigureAuthorization(authorization);
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
        private void ConfigureAuthorization(GameAuthorization authorization)
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
            // Just do nothing...
        }

        private void DefaultRequiredAction(NetIncomingMessage im)
        {
            // Throw an error...
            
        }
        #endregion
    }
}
