using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.MetaData;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Helpers;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Thrusters are ShipParts that can be used to apply thrust to a ship when
    /// controlled by a Player instance.
    /// </summary>
    public class Thruster : ShipPart
    {
        #region Private Fields
        private Boolean _active;
        #endregion

        #region Public Attributes
        public ThrusterData ThrusterData { get; private set; }

        public Vector2 Acceleration { get; private set; }

        public Vector2 WorldAcceleration { get { return Vector2.Transform(this.Acceleration, Matrix.CreateRotationZ(this.MaleConnectionNode.WorldRotation)); } }
        
        public Single WorldRotation { get { return RadianHelper.Normalize(this.MaleConnectionNode.WorldRotation); } }
        #endregion

        #region Constructors
        public Thruster(EntityInfo info, IGame game, string driverHandle = "entity:driver:ship_part") : base(info, game, driverHandle)
        {
            this.ThrusterData = info.Data as ThrusterData;
        }

        public Thruster(long id, EntityInfo info, IGame game, SpriteBatch spriteBatch, string driverHandle = "entity:driver:ship_part") : base(id, info, game, spriteBatch, driverHandle)
        {
            this.ThrusterData = info.Data as ThrusterData;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _active = false;

            this.Acceleration = Vector2.UnitX * this.ThrusterData.Acceleration;
        }
        #endregion

        #region Methods
        public void SetActive(Boolean active)
        {
            _active = active;
            this.UpdateTransformationData();
            this.SetEnabled(true);
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_active && this.Root.IsBridge)
            {
                this.Root.Body.ApplyLinearImpulse(
                    this.WorldAcceleration,
                    this.MaleConnectionNode.WorldPoint);
            }
        }

        internal Boolean MatchesMovementType(MovementType movementType)
        {
            var localAcceleration = Vector2.Transform(this.Acceleration, this.OffsetTranslationMatrix) - Vector2.Transform(this.MaleConnectionNode.LocalPoint, this.OffsetTranslationMatrix);
            var r = RadianHelper.Normalize((float)Math.Atan2(localAcceleration.Y, localAcceleration.X));
            this.Game.Logger.LogCritical(localAcceleration.ToString());
            this.Game.Logger.LogCritical(r.ToString());

            switch (movementType)
            {
                case MovementType.GoForward:
                    return ((r > RadianHelper.PI_HALVES && r < RadianHelper.THREE_PI_HALVES));
                case MovementType.TurnRight:
                    return false;
                case MovementType.GoBackward:
                    return false;
                case MovementType.TurnLeft:
                    return false;
                case MovementType.StrafeRight:
                    return false;
                case MovementType.StrafeLeft:
                    return false;
            }

            return false;
        }
        #endregion
    }
}
