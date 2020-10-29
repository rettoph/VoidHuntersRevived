﻿using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Lidgren.Network;
using System;
using Guppy.Enums;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// Represents a player that is directly controlled
    /// by a user.
    /// </summary>
    public class UserPlayer : Player
    {
        #region Private Fields
        private Group _group;
        private User _user;
        #endregion

        #region Public Properties
        public override String Name => this.User.Name;

        public User User
        {
            get => _user;
            set
            {
                if (this.InitializationStatus >= InitializationStatus.Initializing)
                    throw new Exception("Unable to update UserPlayer User value once initialization has started.");

                _user = value;
            }
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnRead += this.ReadUser;
            this.OnWrite += this.WriteUser;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _group);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnRead -= this.ReadUser;
            this.OnWrite -= this.WriteUser;
        }
        #endregion

        #region Network Methods
        private void WriteUser(NetOutgoingMessage om)
            => om.Write(this.User.Id);

        private void ReadUser(NetIncomingMessage im)
            => this.User = _group.Users.GetById(im.ReadGuid());
        #endregion
    }
}
