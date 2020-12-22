using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.System;
using Guppy.Extensions.Utilities;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Aether;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Utilities.Farseer;

namespace VoidHuntersRevived.Library.Entities
{
    public class Explosion : NetworkEntity
    {
        #region Private Fields
        private Body _body;
        private CircleShape _circle;
        private WorldEntity _world;
        private HashSet<BodyEntity> _contacts;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Queue<BodyEntity> _seperated;
        #endregion

        #region Public Properties
        /// <summary>
        /// The center position of the explosion.
        /// </summary>
        public Vector2 Position
        {
            get => _body.Position;
            set => _body.SetTransformIgnoreContacts(value, 0);
        }

        public Vector2 Velocity
        {
            get => _body.LinearVelocity;
            set => _body.LinearVelocity = value;
        }
        
        /// <summary>
        /// The current radius of the explosion.
        /// </summary>
        public Single Radius => _circle.Radius;

        /// <summary>
        /// The maximum amount of force applied by the explosion.
        /// this value will dissipate as the <see cref="Radius"/>
        /// increases.
        /// </summary>
        public Single Force { get; set; } = 30f;

        /// <summary>
        /// The DPS to be applied on health containing body
        /// entities caught within the explosion.
        /// </summary>
        public Single Damage { get; set; } = 10f;

        /// <summary>
        /// The current age of the explosion.
        /// </summary>
        public Single Age { get; private set; }

        /// <summary>
        /// The maximum explosion age in seconds.
        /// Once this is surpassed the explosion will
        /// no longer apply any impulses.
        /// </summary>
        public Single MaxAge { get; set; } = 7f;

        /// <summary>
        /// The color of the current explosion
        /// </summary>
        public Color Color { get; set; } = Color.White;
        #endregion

        #region Events
        public delegate void ExplosionImpulseAppliedDelegate(Explosion explosion, BodyEntity target, Single force, Vector2 forceVector, Single elapsedSeconds);

        /// <summary>
        /// Event invoked when the current explosion acts upon any
        /// <see cref="BodyEntity"/>
        /// </summary>
        public event ExplosionImpulseAppliedDelegate OnImpulseApplied;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _world);

            _seperated = new Queue<BodyEntity>();
            _contacts = new HashSet<BodyEntity>();
            _body = _world.Live.CreateCircle(15f, 0f).Then(b =>
            { // Set up the explosion body...
                _circle = b.FixtureList[0].Shape as CircleShape;

                b.SleepingAllowed = false;
                b.BodyType = BodyType.Dynamic;

                b.SetIsSensor(true);
                b.SetCollidesWith(Categories.BorderCollidesWith);
                b.SetCollisionCategories(Categories.BorderCollisionCategories);

                b.OnCollision += this.HandleCollision;
                b.OnSeparation += this.HandleSeperation;
            });

            this.Age = 0f;

            this.MessageHandlers[MessageType.Setup].OnWrite += this.WriteData;
            this.MessageHandlers[MessageType.Setup].OnRead += this.ReadData;
        }

        protected override void Release()
        {
            base.Release();

            this.MessageHandlers[MessageType.Setup].OnWrite -= this.WriteData;
            this.MessageHandlers[MessageType.Setup].OnRead -= this.ReadData;

            _body.OnCollision -= this.HandleCollision;
            _body.OnSeparation -= this.HandleSeperation;
            _body.Remove();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Age <= this.MaxAge)
            {
                var seconds = (Single)gameTime.ElapsedGameTime.TotalSeconds;
                var force = this.Force * (1 - (this.Age / this.MaxAge)) * seconds;

                while (_seperated.Any())
                    _contacts.Remove(_seperated.Dequeue());

                _contacts.ForEach(entity =>
                {
                    Vector2 forceVector = this.Position - entity.Position;
                    forceVector *= 1f / (float)Math.Sqrt(forceVector.X * forceVector.X + forceVector.Y * forceVector.Y);
                    forceVector *= force * (float)Math.Pow(entity.live.Mass, 2);
                    forceVector *= -1;

                    entity.ApplyForce(forceVector, this.Position);
                    entity.Nudge();

                    this.OnImpulseApplied?.Invoke(this, entity, force, forceVector, seconds);
                });

                // Increase the radius...
                this.Age += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            }

        }
        #endregion

        #region Event Handlers
        private bool HandleCollision(Fixture sender, Fixture other, Contact contact)
        {
            if(other.Tag is BodyEntity entity)
            {
                _contacts.Add(entity);

                return true;
            }

            return false;
        }

        private void HandleSeperation(Fixture sender, Fixture other, Contact contact)
        {
            if (other.Tag is BodyEntity entity && !_seperated.Any(e => e == entity))
            {
                _seperated.Enqueue(entity);
            }
        }
        #endregion

        #region Network Methods
        private void WriteData(NetOutgoingMessage om)
        {
            om.Write(this.Position);
            om.Write(this.Velocity);
            om.Write(this.Color);
            om.Write(this.MaxAge);
            om.Write(this.Damage);
        }

        private void ReadData(NetIncomingMessage im)
        {
            this.Position = im.ReadVector2();
            this.Velocity = im.ReadVector2();
            this.Color = im.ReadColor();
            this.MaxAge = im.ReadSingle();
            this.Damage = im.ReadSingle();
        }
        #endregion
    }
}
