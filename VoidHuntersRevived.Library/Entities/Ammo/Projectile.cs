using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.Ammo
{
    /// <summary>
    /// Projectile's represent ammo that moves
    /// and does not instantly cast a forward ray.
    /// 
    /// Projectiles thus, must have amoving body.
    /// </summary>
    public class Projectile : Entity
    {
        #region Private Fields
        private Double _life;
        private World _world;

        /// <summary>
        /// The projectiles main body
        /// </summary>
        private Body _body;

        /// <summary>
        /// The body any incoming data is read into.
        /// 
        /// By default, the is the _body, but can
        /// be overwritten with the SetReadBody method
        /// </summary>
        private Body _readBody;

        #endregion

        #region Public Attributes
        public Gun Gun { get; internal set; }

        public Body Body { get => _body; }

        public Vector2 Position { get => _body.Position; set => _body.SetTransform(value, 0); }

        public Vector2 LinearVelocity { get => _body.LinearVelocity; set => _body.LinearVelocity = value; }
        #endregion

        #region Constructor
        public Projectile(World world)
        {
            _world = world;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.SetEnabled(false);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _life = 0;
            _body = this.BuildBody(_world);
            _readBody = _body;
        }

        public override void Dispose()
        {
            base.Dispose();

            _body.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _life += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_life > 5000)
                this.Dispose();
        }
        #endregion

        #region Farseer Methods
        protected Body BuildBody(World world)
        {
            var body = BodyFactory.CreateCircle(world, 0.1f, 0.1f, this.Gun.WorldBodyAnchor, BodyType.Dynamic);
            body.IsSensor = true;
            body.IsBullet = true;

            // The target radian the gun will shoot towards
            var target = this.Gun.Rotation + this.Gun.JointAngle;
            // The difference between the current velocity and the guns target radian
            var offset = (Single)Math.Atan2(this.Gun.Root.LinearVelocity.Y, this.Gun.Root.LinearVelocity.X) - target;
            // The amount of forward thrust the bullet should be given, as a multiplier of the gun's current velocity
            var boost = (Single)Math.Cos(offset + MathHelper.Pi);
            // An inverse 0 through 1, used for the true modifier
            var modifier = 0.5f * (Single)Math.Cos(2 * offset + MathHelper.Pi) + 0.5f;

            // Set the bullets brand new velocity.. complete with the velocity multiplier
            body.LinearVelocity = (this.Gun.Root.LinearVelocity * modifier) + Vector2.Transform(Vector2.UnitX * (-10 - (this.Gun.Root.LinearVelocity.Length() * boost)), Matrix.CreateRotationZ(target));

            return body;
        }
        #endregion

        #region Set Methods
        public void SetReadBody(Body body)
        {
            _readBody = body;
        }

        public void ReadVitals(Vector2 position, Vector2 linearVeolocity)
        {
            _readBody.SetTransform(position, 0);
            _readBody.LinearVelocity = linearVeolocity;
        }
        #endregion
    }
}
