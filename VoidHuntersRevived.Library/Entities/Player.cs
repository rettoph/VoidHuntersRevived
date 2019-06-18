﻿using Guppy;
using Guppy.Configurations;
using Guppy.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using System.Linq;
using Lidgren.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Collections;
using Guppy.Network.Security;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using Guppy.Network.Groups;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Configurations;
using FarseerPhysics.Dynamics;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// the payer class represents an in game instance that contains 
    /// </summary>
    public class Player : NetworkEntity
    {
        #region Private Attributes
        private Dictionary<Direction, Boolean> _directions;
        #endregion

        #region Public Attributes
        public User User { get; private set; }
        public ShipPart Bridge { get; private set; }
        public TractorBeam TractorBeam { get; private set; }
        #endregion

        #region Events
        public event EventHandler<User> OnUserUpdated;
        public event EventHandler<FarseerEntity> OnBridgeUpdated;
        public event EventHandler<Direction> OnDirectionUpdated;
        public event EventHandler<PlayerInstanceInternalsConfiguration> OnInitializatingInternals;
        #endregion

        #region Constructors
        public Player(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public Player(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            _directions = ((Direction[])Enum.GetValues(typeof(Direction)))
                .ToDictionary(
                keySelector: d => d,
                elementSelector: d => false);

            this.SetUpdateOrder(100);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var internals = new PlayerInstanceInternalsConfiguration();
            this.OnInitializatingInternals?.Invoke(this, internals);

            this.TractorBeam = internals.TractorBeam;
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Bridge != null)
            {
                var thrust = Vector2.Transform(new Vector2(0.25f, 0), Matrix.CreateRotationZ(this.Bridge.Rotation));

                if (_directions[Direction.Forward])
                    this.Bridge.ApplyLinearImpulse(thrust);
                if (_directions[Direction.Backward])
                    this.Bridge.ApplyLinearImpulse(-thrust);

                if (_directions[Direction.TurnLeft])
                    this.Bridge.ApplyAngularImpulse(-0.025f);
                if (_directions[Direction.TurnRight])
                    this.Bridge.ApplyAngularImpulse(0.025f);
            }
        }
        #endregion

        #region Utility Methods
        public void SetUser(User user)
        {
            if (user != this.User)
            {
                this.logger.LogDebug($"Setting Player({this.Id}) user to User({user.Id})");

                this.User = user;

                this.Dirty = true;
                this.OnUserUpdated?.Invoke(this, user);
            }
        }

        public void SetBridge(ShipPart bridge)
        {
            if (bridge != this.Bridge)
            {
                this.logger.LogDebug($"Setting Player({this.Id}) bridge to {bridge.GetType().Name}({bridge.Id})");

                this.Bridge = bridge;
                this.Bridge.BridgeFor = this;
                this.Bridge.CollidesWith = Category.Cat1 | Category.Cat2;

                this.Dirty = true;
                this.OnBridgeUpdated?.Invoke(this, bridge);
            }
        }

        public void UpdateDirection(Direction direction, Boolean value)
        {
            if (_directions[direction] != value)
            {
                this.logger.LogDebug($"Updating Player({this.Id}) Direction<{direction}> to {value}.");

                _directions[direction] = value;

                this.OnDirectionUpdated?.Invoke(this, direction);
            }
        }
        public Boolean GetDirection(Direction direction)
        {
            return _directions[direction];
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
                this.SetUser((this.scene as VoidHuntersWorldScene).Group.Users.GetById(im.ReadGuid()));
            else
                this.SetUser(null);

            if (im.ReadBoolean())
                this.SetBridge(this.entities.GetById(im.ReadGuid()) as ShipPart);
            else
                this.SetBridge(null);

            this.TractorBeam = this.entities.GetById(im.ReadGuid()) as TractorBeam;
        }

        protected override void write(NetOutgoingMessage om)
        {
            if (this.User == null)
                om.Write(false);
            else
            {
                om.Write(true);
                om.Write(this.User.Id);
            }

            if (this.Bridge == null)
                om.Write(false);
            else
            {
                om.Write(true);
                om.Write(this.Bridge.Id);
            }

            om.Write(this.TractorBeam.Id);
        }
        #endregion
    }
}
