﻿using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Guppy.Interfaces;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.Events.Delegates;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart : BodyEntity
    {
        #region Private Fields
        private Ship _ship;
        private Controller _controller;
        #endregion

        #region Public Properties
        public Ship Ship
        {
            get => _ship;
            internal set
            {
                if(value != _ship)
                {
                    var old = _ship;
                    _ship = value;

                    this.OnShipChanged?.Invoke(this, old, _ship);
                }
            }
        }

        public Controller Controller
        {
            get => _controller;
            internal set => this.OnControllerChanged.InvokeIfChanged(_controller != value, this, ref _controller, value);
        }

        public ShipPartConfiguration Configuration { get; set; }
        public Boolean IsRoot => !this.MaleConnectionNode.Attached;
        public ShipPart Root => this.Chain.Root;
        public ShipPart Parent => this.IsRoot ? null : this.MaleConnectionNode.Target.Parent;

        public Color Color => this.Root.Ship == default(Ship) ? this.Root.Configuration.DefaultColor : this.Ship.Color;
        #endregion

        #region Events
        public event OnChangedEventDelegate<ShipPart, Controller> OnControllerChanged;
        public event OnChangedEventDelegate<ShipPart, Ship> OnShipChanged;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnChainChanged += this.HandleChainChanged;
            this.OnControllerChanged += this.HandleControllerChanged;
            this.ValidateWritePosition += this.HandleValidateWritePosition;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Transformations_PreInitialize(provider);

            this.BodyType = BodyType.Dynamic;

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            // Run _before_ Driven.Initialize to beat the Driver configuration.
            this.ConnectionNode_Initialize(provider);
            this.Chain_Initialize(provider);

            base.Initialize(provider);

            // Clean the chain once the ship part is initialized
            this.AngularDamping = 0.5f;
            this.LinearDamping = 0.5f;
        }

        protected override void Release()
        {
            base.Release();

            this.Transformations_Release();
            this.ConnectionNode_Release();
            this.Chain_Release();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnChainChanged -= this.HandleChainChanged;
            this.OnControllerChanged -= this.HandleControllerChanged;
            this.ValidateWritePosition -= this.HandleValidateWritePosition;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Reconfigure some internal actions when a ship part is added into a
        /// brand new chain...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(old != default(Chain))
            { // Unset old chain actions as needed...
                // Remove the draw handler from the old chain
                old.OnDraw -= this.TryDraw;
            }

            if(value != default(Chain))
            { // Set new chain actions as needed...
                // When the root chain is drawn, we should draw the current ship part as well
                value.OnDraw += this.TryDraw;

                // Flush the default chain values to the ShipPart...
                this.Ship = value.Ship;
                this.Controller = value.Controller;
                // this.Authorization = value.Controller.Authorization;
            }
        }

        /// <summary>
        /// Configure internal attributes and event when the entities
        /// controller value is updated...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void HandleControllerChanged(ShipPart sender, Controller old, Controller value)
            => this.Authorization = value?.Authorization ?? this.settings.Get<GameAuthorization>();

        private bool HandleValidateWritePosition(BodyEntity sender, GameTime args)
            => this.IsRoot;
        #endregion
    }
}
