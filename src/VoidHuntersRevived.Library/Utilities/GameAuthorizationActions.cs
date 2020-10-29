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
        private Dictionary<NetworkAuthorization, Action<NetIncomingMessage>> _actions;
        private Action<NetIncomingMessage> _action;
        private Action<NetIncomingMessage> _defaultAction;
        #endregion

        #region Public Fields
        public readonly String Type;
        #endregion

        #region Constructor
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="type">The name of the action beinf represented within the current contained.</param>
        /// <param name="defaultAction">The default action to run if none is defined.</param>
        /// <param name="actions">A map of which actions to preform based on a GameAuthorization index.</param>
        internal GameAuthorizationActions(
            String type,
            Action<NetIncomingMessage> defaultAction,
            Dictionary<NetworkAuthorization, Action<NetIncomingMessage>> actions)
        {
            _actions = actions;
            _defaultAction = defaultAction;

            this.Type = type;
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
        public void ConfigureAuthorization(NetworkAuthorization authorization)
        {
            if (_actions.ContainsKey(authorization))
                _action = _actions[authorization];
            else
                _action = this.DefaultOptionalAction;
        }

        private void DefaultOptionalAction(NetIncomingMessage im)
            => _defaultAction?.Invoke(im);
        #endregion
    }
}
