using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Guppy.Interfaces;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart : BodyEntity
    {
        #region Private Fields
        private Ship _ship;
        private Controller _controller;
        private ChunkManager _chunks;
        #endregion

        #region Public Attributes
        public Ship Ship
        {
            get => this.IsRoot ? _ship : this.Root.Ship;
            internal set
            {
                if (this.IsRoot)
                {
                    if (_ship != value)
                    {
                        var old = _ship;
                        _ship = value;
                        this.OnShipChanged?.Invoke(this, old, _ship);
                    }
                }
                else
                    throw new Exception("Unable to update Ship of child ShipPart.");
            }
        }
        public Controller Controller
        {
            get => this.IsRoot ? _controller : this.Root.Controller;
            internal set
            {
                if (this.IsRoot)
                {
                    if(_controller != null)
                        _controller.OnAuthorizationChanged -= this.HandleControllerAuthorizationChanged;

                    _controller = value;

                    if (_controller != null)
                        _controller.OnAuthorizationChanged += this.HandleControllerAuthorizationChanged;

                    this.OnControllerChanged?.Invoke(this, _controller);
                }
                else
                    throw new Exception("Unable to update Controller of child ShipPart.");
            }
        }

        public ShipPartConfiguration Configuration { get; set; }
        public Boolean IsRoot => !this.MaleConnectionNode.Attached;
        public ShipPart Root => this.IsRoot ? this : this.Parent.Root;
        public ShipPart Parent => this.IsRoot ? null : this.MaleConnectionNode.Target.Parent;

        public Color Color => this.Root.Ship == default(Ship) ? this.Root.Configuration.DefaultColor : this.Ship.Color;
        #endregion

        #region Events
        public event GuppyEventHandler<ShipPart, Controller> OnControllerChanged;
        public event GuppyDeltaEventHandler<ShipPart, Ship> OnShipChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);

            this.Transformations_PreInitialize(provider);

            this.BodyType = BodyType.Dynamic;

            this.Enabled = false;
            this.Visible = false;

            this.OnRootChanged += this.HandleRootChanged;
            this.OnChainCleaned += this.HandleChainCleaned;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            // Run _before_ Driven.Initialize to beat the Driver configuration.
            this.ConnectionNode_Initialize(provider);

            base.Initialize(provider);

            // Add the ship part to the chunk manager by default...
            _chunks.TryAdd(this);

            // Clean the chain once the ship part is initialized
            this.CleanChain(DirtyChainType.Both);
            this.AngularDamping = 0.5f;
            this.LinearDamping = 0.5f;

            this.MaleConnectionNode.OnDetached += this.HandleMaleConnectionNodeDetached;
            this.OnControllerChanged += this.HandleControllerChanged;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.Transformations_Dispose();
            this.ConnectionNode_Dispose();

            this.OnRootChanged -= this.HandleRootChanged;
            this.OnChainCleaned -= this.HandleChainCleaned;
            this.MaleConnectionNode.OnDetached -= this.HandleMaleConnectionNodeDetached;
            this.OnControllerChanged -= this.HandleControllerChanged;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Simple handler that will map the internal draw methods
        /// of ship parts so that when the root part is drawn
        /// all children will also be drawn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void HandleRootChanged(ShipPart sender, ShipPart old, ShipPart value)
        {
            if (old != null && old != this)
            {
                old.OnDraw -= this.TryDraw;
            }

            if (value != this)
            {
                // Unset any saved ship value...
                _ship?.SetBridge(null);
                _ship = null;

                value.OnDraw += this.TryDraw;
            }
        }

        private void HandleChainCleaned(ShipPart sender, DirtyChainType arg)
        {
            if(!this.IsRoot)
            {
                // Uset any saved controller value
                _controller?.TryRemove(this);
                _controller = null;
            }
        }

        private void HandleMaleConnectionNodeDetached(ConnectionNode sender, ConnectionNode arg)
        {
            // Automatically add ShipPart to the chunk manager on male detach...
            _chunks.TryAdd(this);
        }


        /// <summary>
        /// Configure internal attributes and event when the entities
        /// controller value is updated...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="controller"></param>
        private void HandleControllerChanged(ShipPart sender, Controller controller)
            => this.Authorization = controller?.Authorization ?? this.settings.Get<GameAuthorization>();

        private void HandleControllerAuthorizationChanged(Controller sender, GameAuthorization old, GameAuthorization value)
            => this.Authorization = this.Controller.Authorization;
        #endregion
    }
}
