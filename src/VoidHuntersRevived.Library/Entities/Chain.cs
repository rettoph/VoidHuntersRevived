using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using VoidHuntersRevived.Library.Entities.Controllers;
using Guppy.Extensions.System.Collections;
using log4net.Repository.Hierarchy;
using log4net;
using Guppy.Extensions.log4net;
using Guppy.Events.Delegates;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Xna.Framework;
using Guppy.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    public class Chain : Entity
    {
        #region Private Fields
        private Ship _ship;
        private Controller _controller;
        private ShipPart _root;
        #endregion

        #region Public Properties
        public ShipPart Root
        {
            get => _root;
            private set => this.Add(_root = value);
        }

        public Ship Ship
        {
            get => _ship;
            internal set
            {
                this.OnShipChanged.InvokeIf(_ship != value, this, ref _ship, value);
            }
        }

        public Controller Controller
        {
            get => _controller;
            internal set
            {
                this.OnControllerChanged.InvokeIf(_controller != value, this, ref _controller, value);
            }
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Chain, Controller> OnControllerChanged;
        public event OnChangedEventDelegate<Chain, Ship> OnShipChanged;
        public event OnEventDelegate<Chain, ShipPart> OnShipPartAdded;
        public event OnEventDelegate<Chain, ShipPart> OnShipPartRemoved;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Root.OnStatus[ServiceStatus.PostReleasing] += this.HandleRootPostReleasing;

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Release()
        {
            base.Release();

            this.Root.OnStatus[ServiceStatus.PostReleasing] -= this.HandleRootPostReleasing;

            this.Ship = null;
            this.Root = null;
            this.Controller = null;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Root.TryUpdate(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Remove an item from the chain and add into another chain.
        /// </summary>
        /// <param name="shipPart"></param>
        /// <param name="into"></param>
        public Boolean Remove(ShipPart shipPart, Chain into)
        {
            // Set the chain to null...
            shipPart.Chain = into;
            this.OnShipPartRemoved?.Invoke(this, shipPart);

            // There is nothing left in the chain, so self release...
            if (shipPart == this.Root && into != this && this.Status == ServiceStatus.Ready)
                this.TryRelease();

            return true;
        }

        internal void Add(ShipPart shipPart)
        {
            if (shipPart == default)
                return;

            foreach(ShipPart sp in shipPart.Items())
            {
                // Remove from old chain (if any)...
                if (!sp.Chain?.Remove(shipPart, this) ?? true)
                    sp.Chain = this; // Set the internal chain values...
            }

            // Invoke the ShipPartAdded event once.
            this.OnShipPartAdded?.Invoke(this, shipPart);
        }
        #endregion

        #region Event Handlers
        private void HandleRootPostReleasing(IService sender)
            => this.TryRelease();
        #endregion

        #region Static Methods
        public static Chain Create(ServiceProvider provider, ShipPart root)
            => provider.GetService<Chain>((chain, p, d) =>
            { // Configure the new chain...
                chain.Root = root;
                chain.Add(root);

                // Add the new chain into the chunk manager by default
                provider.GetService<ChunkManager>().TryAdd(chain);
            });
        #endregion
    }
}
