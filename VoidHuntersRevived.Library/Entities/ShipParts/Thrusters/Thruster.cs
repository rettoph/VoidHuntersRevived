using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.MetaData;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Helpers;
using VoidHuntersRevived.Core.Extensions;

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

        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _origin;
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

            _spriteBatch = spriteBatch;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _active = false;

            this.Acceleration = Vector2.UnitX * this.ThrusterData.Acceleration;

            // If there is a spritebatch defined, load the thruster content
            if(_spriteBatch != null)
            {
                var contentLoader = this.Game.Provider.GetLoader<ContentLoader>();

                _texture = contentLoader.Get<Texture2D>("texture:thruster_overlay:active");
                _origin = new Vector2(0, (float)_texture.Height / 2);

                // Make the current thruster visible
                this.SetVisible(true);
            }
        }
        #endregion

        #region Methods
        public void SetActive(Boolean active)
        {
            _active = active;
            this.UpdateTransformationData();
            this.SetEnabled(true);
        }

        internal Boolean MatchesMovementType(MovementType movementType)
        {
            // Each andle of movement has a buffer sone on inclusivity
            var buffer = 0.01f;

            // The chain's center of mass
            var com = this.Root.Body.LocalCenter;
            // The point acceleration is applied
            var ap = Vector2.Transform(this.MaleConnectionNode.LocalPoint, this.OffsetTranslationMatrix);
            // The point acceleration is targeting
            var at = Vector2.Transform(this.Acceleration + this.MaleConnectionNode.LocalPoint, this.OffsetTranslationMatrix);

            // The angle between the com and the acceleration point
            var apr = RadianHelper.Normalize((float)Math.Atan2(ap.Y - com.Y, ap.X - com.X));
            // The angle between the com and the acceleration target
            var atr = RadianHelper.Normalize((float)Math.Atan2(at.Y - com.Y, at.X - com.X));
            // The angle between the acceleration point and the acceleration target
            var apatr = RadianHelper.Normalize((float)Math.Atan2(at.Y - ap.Y, at.X - ap.X));
            // The relative acceleration target rotation between the acceleration point and center of mass
            var ratr = RadianHelper.Normalize(apatr - apr);

            var apatr_lower = apatr - buffer;
            var apatr_upper = apatr + buffer;

            switch (movementType)
            {
                case MovementType.GoForward:
                    return (apatr_lower > RadianHelper.PI_HALVES && apatr_upper < RadianHelper.THREE_PI_HALVES);
                case MovementType.TurnRight:
                    return ratr > RadianHelper.PI && ratr < RadianHelper.TWO_PI;
                case MovementType.GoBackward:
                    return (apatr >= 0 && apatr_upper < RadianHelper.PI_HALVES) || (apatr_lower >= RadianHelper.THREE_PI_HALVES && apatr < RadianHelper.TWO_PI);
                case MovementType.TurnLeft:
                    return ratr > 0 && ratr < RadianHelper.PI;
                case MovementType.StrafeRight:
                    return (apatr_lower > 0 && apatr_upper < RadianHelper.PI);
                case MovementType.StrafeLeft:
                    return (apatr_lower > RadianHelper.PI && apatr_upper < RadianHelper.TWO_PI);
            }

            return false;
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

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(_active)
                _spriteBatch.Draw(
                    texture: _texture,
                    position: this.MaleConnectionNode.WorldPoint,
                    sourceRectangle: _texture.Bounds,
                    color: Color.White,
                    rotation: this.MaleConnectionNode.WorldRotation + RadianHelper.PI,
                    origin: _origin,
                    scale: new Vector2(0.035f, 0.01f),
                    effects: SpriteEffects.None,
                    layerDepth: 0);
        }
        #endregion
    }
}
