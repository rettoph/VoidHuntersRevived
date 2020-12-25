using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using Guppy.Enums;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// The base player class. By defualt, there are 2 main types
    /// of players. Human and Computers. Players are classes
    /// that can take control of a ship instance.
    /// </summary>
    public abstract class Player : NetworkEntity
    {
        #region private Fields
        private EntityList _entities;
        private Ship _ship;
        #endregion

        #region Protected Fields
        protected ServiceList<Player> players { get; private set; }
        #endregion

        #region Public Properties
        public abstract String Name { get; }
        public Ship Ship
        {
            get => _ship;
            set
            {
                if (this.Status >= ServiceStatus.Initializing && this.Status != ServiceStatus.Releasing)
                    throw new Exception("Unable to update Player Ship value once initialization has started.");

                _ship = value;
                if (_ship != null)
                    _ship.Player = this;
            }
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.MessageHandlers[MessageType.Create].Add(this.ReadShip, this.WriteShip);
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);

            this.players = provider.GetService<ServiceList<Player>>();
            this.players.TryAdd(this);
        }

        protected override void Release()
        {
            base.Release();

            // Unset the old ship value...
            this.Ship = null;
            this.players.TryRemove(this);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.MessageHandlers[MessageType.Create].Add(this.ReadShip, this.WriteShip);
        }
        #endregion

        #region Network Methods
        private void ReadShip(NetIncomingMessage im)
            => this.Ship = im.ReadEntity<Ship>(_entities);

        private void WriteShip(NetOutgoingMessage om)
            => om.Write(this.Ship);
        #endregion
    }
}
