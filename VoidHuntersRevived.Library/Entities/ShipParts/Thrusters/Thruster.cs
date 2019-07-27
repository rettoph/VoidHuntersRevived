using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Guppy.Configurations;
using Guppy.Loaders;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// Thrusters are basic ship parts
    /// that can apply thrust to themselves
    /// (and thus any chain they are currently
    /// connected to)
    /// </summary>
    public class Thruster : RigidShipPart
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _thrust;
        private Texture2D _line;

        #region Public Attributes
        public Vector2 Thrust { get { return Vector2.Transform(this.ThrustStrength, Matrix.CreateRotationZ(this.Rotation)); } }
        public Vector2 LocalThrust { get { return Vector2.Transform(this.ThrustStrength, Matrix.CreateRotationZ(this.LocalRotation)); } }
        public Vector2 ThrustStrength { get; protected set; }
        public Boolean Active { get; private set; }
        #endregion

        #region Constructors
        public Thruster(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public Thruster(Guid id, ContentLoader content, SpriteBatch spriteBatch, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
            _spriteBatch = spriteBatch;
            _thrust = content.Get<Texture2D>("texture:thrust");
            _line = content.Get<Texture2D>("texture:thrust:line");
        }
        #endregion

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            if(this.ThrustStrength == default(Vector2))
                this.ThrustStrength = new Vector2(1000, 0);

            this.Active = false;
        }
        #endregion

        #region Frame Methods
        protected override void draw(GameTime gameTime)
        {
            base.draw(gameTime);

            // _spriteBatch.Draw(
            //     texture: _line,
            //     position: this.WorldCenteroid,
            //     sourceRectangle: _line.Bounds,
            //     color: Color.White,
            //     rotation: this.Rotation,
            //     origin: _line.Bounds.Center.ToVector2(),
            //     scale: 0.01f,
            //     effects: SpriteEffects.None,
            //     layerDepth: 0);

            if(this.Active)
            {
                _spriteBatch.Draw(
                    texture: _thrust,
                    position: this.WorldCenteroid,
                    sourceRectangle: _thrust.Bounds,
                    color: Color.White,
                    rotation: this.Rotation + MathHelper.Pi,
                    origin: new Vector2(0, _thrust.Bounds.Center.ToVector2().Y),
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }

        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if(this.Active)
            {
                if (!this.Root.IsBridge)
                    this.SetActive(false);
                else
                    this.Root.ApplyForce(this.Thrust * ((Single)gameTime.ElapsedGameTime.Milliseconds / 1000), this.WorldCenteroid);
            }
        }
        #endregion

        #region Set Methods
        internal void SetActive(Boolean value)
        {
            this.Active = value;
        }
        #endregion

        #region Helper Methods
        internal Boolean MatchesMovementType(Direction direction)
        {
            // Each andle of movement has a buffer sone on inclusivity
            var buffer = 0.01f;

            // The chain's center of mass
            var com = this.Root.LocalCenter;
            // The point acceleration is applied
            var ap = Vector2.Transform(this.Centeroid, this.LocalTransformation);
            // The point acceleration is targeting
            var at = ap + this.LocalThrust;

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

            this.logger.LogDebug($"Thruster({this.Id}) => {direction}: {apatr}, {apatr_upper}, {apatr_lower}");

            switch (direction)
            {
                case Direction.Forward:
                    return (apatr_upper < MathHelper.PiOver2 || apatr_lower > 3 * MathHelper.PiOver2);
                case Direction.TurnRight:
                    return ratr > 0 && ratr < MathHelper.Pi;
                case Direction.Backward:
                    return apatr_lower > MathHelper.PiOver2 && apatr_upper < 3 * MathHelper.PiOver2;
                case Direction.TurnLeft:
                    return ratr > MathHelper.Pi && ratr < MathHelper.TwoPi;

                // case Direction.StrafeRight:
                //     return (apatr_lower > 0 && apatr_upper < RadianHelper.PI);
                // case Direction.StrafeLeft:
                //     return (apatr_lower > RadianHelper.PI && apatr_upper < RadianHelper.TWO_PI);
            }

            return false;
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            base.read(im);

            this.ThrustStrength = im.ReadVector2();
        }

        protected override void write(NetOutgoingMessage om)
        {
            base.write(om);

            om.Write(this.ThrustStrength);
        }
        #endregion

        #region Import & Export Methods
        protected internal override void Export(BinaryWriter writer)
        {
            base.Export(writer);

            writer.Write(this.ThrustStrength.X);
            writer.Write(this.ThrustStrength.Y);
        }

        protected internal override void Import(BinaryReader reader)
        {
            base.Import(reader);

            this.ThrustStrength = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
        #endregion
    }
}
