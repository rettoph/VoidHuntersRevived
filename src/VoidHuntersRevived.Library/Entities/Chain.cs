using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using VoidHuntersRevived.Library.Entities.Controllers;
using Guppy.Extensions.Collections;
using log4net.Repository.Hierarchy;
using log4net;
using Guppy.IO.Extensions.log4net;
using Guppy.Events.Delegates;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities
{
    public class Chain : Entity
    {
        #region Private Fields
        private ServiceProvider _provider;
        private ILog _logger;
        private Ship _ship;
        private Controller _controller;
        private ShipPart _root;
        private NetworkAuthorization _authorization;
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
                this.OnShipChanged.InvokeIfChanged(_ship != value, this, ref _ship, value);
                this.Do(sp => sp.Ship = value);
            }
        }

        public Controller Controller
        {
            get => _controller;
            internal set
            {
                this.OnControllerChanged.InvokeIfChanged(_controller != value, this, ref _controller, value);
                this.Do(sp => sp.Controller = value);
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

            _provider = provider;
            _authorization = provider.GetService<Settings>().Get<NetworkAuthorization>();
            provider.Service(out _logger);

            _logger.Verbose(() => $"Created new Chain bound to ShipPart<{this.Root.ServiceConfiguration.Name}>({this.Root.Id}).");

            this.Enabled = false;
            this.Visible = false;
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
        internal void Remove(ShipPart shipPart)
        {
            if(shipPart == this.Root)
            { // There is nothing left in the chain, so self release...
                this.TryRelease();
            }
            else
            { // The ShipPart to remove is a child, add it into a new chain of its own...
                Chain.Create(_provider, shipPart);
            }

            this.OnShipPartRemoved?.Invoke(this, shipPart);
        }

        internal void Add(ShipPart shipPart)
        {
            shipPart.Chain = this;
            this.OnShipPartAdded?.Invoke(this, shipPart);
        }

        /// <summary>
        /// Recersively call an action preformed on ALL
        /// internal chain ShipPart items.
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action<ShipPart> action)
            => this.Do(this.Root, action);

        private void Do(ShipPart target, Action<ShipPart> action)
        {
            action.Invoke(target);

            target.Children.ForEach(child => this.Do(child, action));
        }
        #endregion

        #region Static Methods
        public static Chain Create(ServiceProvider provider, ShipPart root)
            => provider.GetService<Chain>((chain, p, d) =>
            { // Configure the new chain...
                chain.Root = root;
                root.Chain = chain;

                // Add the new chain into the chunk manager by default
                provider.GetService<ChunkManager>().TryAdd(chain);
            });
        #endregion
    }
}
