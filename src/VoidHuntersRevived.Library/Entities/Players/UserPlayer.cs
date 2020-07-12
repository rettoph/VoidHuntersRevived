using Guppy.DependencyInjection;
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
            set
            {
                if (this.User != value)
                {
                    _user = value;
                    this.OnUserChanged?.Invoke(this, _user);
                }
            }
        }
        public override String Name => this.User?.Name;
        #endregion

        #region Events
        public event GuppyEventHandler<UserPlayer, User> OnUserChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);
        }
        #endregion
    }
}
