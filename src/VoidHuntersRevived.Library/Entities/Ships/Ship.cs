using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Enums;
using Guppy.EntityComponent.Interfaces;
using Guppy.Events.Delegates;
using Guppy.Extensions.System;
using Guppy.Interfaces;
using Guppy.Network;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Entities.Ships
{
    public class Ship : MagicNetworkLayerable
    {
        #region Private Fields
        private Player _player;
        private Chain _chain;
        #endregion

        #region Public Properties
        public Player Player
        {
            get => _player;
            set
            {
                this.OnPlayerChanged.InvokeIf(_player != value, this, ref _player, value);

                if (value != default && value.Ship != this)
                    value.Ship = this;
            }
        }

        public Chain Chain
        {
            get => _chain;
            set
            {
                if (this.Status != ServiceStatus.Initializing)
                    throw new Exception("Unable to update Ship.Chain after initialization.");

                _chain = value;
            }
        }

        public override Pipe Pipe
        {
            get => this.Chain.Pipe;
            protected set
            {
                // Nothing to see here
            }
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Ship, Player> OnPlayerChanged;

        public override event OnChangedEventDelegate<IMagicNetworkEntity, Pipe> OnPipeChanged
        {
            add => this.Chain.OnPipeChanged += value;
            remove => this.Chain.OnPipeChanged -= value;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.LayerGroup = LayersContexts.Ships.Group.GetValue();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Chain.Color = Color.Cyan;

            this.Chain.Corporeal = true;
            this.Chain.OnStatusChanged += this.HandleChainStatusChanged;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Chain.OnStatusChanged -= this.HandleChainStatusChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleChainStatusChanged(IService sender, ServiceStatus old, ServiceStatus value)
        {
            if(value == ServiceStatus.Uninitializing)
            {
                this.Dispose();
            }
        }
        #endregion
    }
}
