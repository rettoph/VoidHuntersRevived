using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Components;
using Guppy.Network.Contexts;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    public abstract class UserPlayerCurrentUserCommandActionBaseComponent<TAction> : UserPlayerCurrentUserActionBaseComponent<TAction>
    {
        #region Private Fields
        private CommandService _commands;
        #endregion

        #region Public Properties
        /// <summary>
        /// The command to bind the <see cref="CurrentUserCommandHandler"/> to.
        /// </summary>
        public abstract String CurrentUserCommandInput { get; }

        /// <summary>
        /// Handler for the defined <see cref="CurrentUserCommandHandler"/>. This should parse incoming arguments/options
        /// and invoke <see cref="TryDoActionRequest(TAction)"/>.
        /// </summary>
        public abstract ICommandHandler CurrentUserCommandHandler { get; }
        #endregion

        #region Lifecycle Methods
        protected override void InitializeCurrentUser(GuppyServiceProvider provider)
        {
            base.InitializeCurrentUser(provider);

            provider.Service(out _commands);

            _commands.Get<Command>(this.CurrentUserCommandInput).Handler = this.CurrentUserCommandHandler;
        }

        protected override void ReleaseCurrentUser()
        {
            base.ReleaseCurrentUser();

            _commands.Get<Command>(this.CurrentUserCommandInput).Handler = default;

            _commands = default;
        }
        #endregion
    }
}
