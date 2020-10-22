using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Interfaces;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// Represents a player that is directly controlled
    /// by a user.
    /// </summary>
    public class UserPlayer : Player
    {
        #region Private Fields
        private User _user;
        #endregion

        #region Public Attributes
        public User User
        {
            get => _user;
            set => this.OnUserChanged.InvokeIfChanged(value != _user, this, ref _user, value);
        }
        public override String Name => this.User?.Name;
        #endregion

        #region Events
        public event OnChangedEventDelegate<UserPlayer, User> OnUserChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);
        }
        #endregion
    }
}
