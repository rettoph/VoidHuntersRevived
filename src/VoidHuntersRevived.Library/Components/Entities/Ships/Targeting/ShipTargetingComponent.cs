using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.DependencyInjection;
using Guppy.Network.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    public abstract class ShipTargetingComponent : RemoteHostComponent<Ship>
    {
        #region Private Fields
        private Vector2 _target;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current ship's world position target.
        /// </summary>
        public Vector2 Target
        {
            get => _target;
            set => this.OnTargetChanged.InvokeIf(_target != value, this.Entity, ref _target, value);
        }
        #endregion

        #region Events
        /// <summary>
        /// A simple event invoked when the <see cref="Target"/> property
        /// is updated.
        /// </summary>
        public event OnEventDelegate<Ship, Vector2> OnTargetChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(Constants.Messages.Ship.TargetChanged, Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);
        }
        #endregion
    }
}
