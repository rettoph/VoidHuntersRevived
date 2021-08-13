using Guppy.Enums;
using Guppy.Events.Delegates;
using Guppy.Network;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Entities.Ships
{
    public class Ship : NetworkEntity
    {
        #region Private Fields
        private Player _player;
        private Chain _chain;
        #endregion

        #region Public Properties
        public Player Player
        {
            get => _player;
            set => this.OnPlayerChanged.InvokeIf(_player != value, this, ref _player, value);
        }

        public Chain Chain
        {
            get => _chain;
            set
            {
                if (this.Status != ServiceStatus.PreInitializing)
                    throw new Exception("Unable to update Ship.Chain after initialization.");

                _chain = value;
            }
        }

        public override IPipe Pipe
        {
            get => this.Chain.Pipe;
            protected set => throw new NotImplementedException();
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Ship, Player> OnPlayerChanged;

        public override event OnChangedEventDelegate<INetworkEntity, IPipe> OnPipeChanged
        {
            add => this.Chain.OnPipeChanged += value;
            remove => this.Chain.OnPipeChanged -= value;
        }
        #endregion
    }
}
