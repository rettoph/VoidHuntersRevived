using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities.Farseer;
using VoidHuntersRevived.Library.Extensions.Aether;
using Lidgren.Network;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.Events.Delegates;
using System.Linq;
using Guppy.Extensions.System;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;

namespace VoidHuntersRevived.Library.Entities
{
    public class BodyEntity : AetherChildEntity<Body, World>
    {
        #region Static Attributes
        /// <summary>
        /// The amount a slave body should lerp towards the master
        /// per second
        /// </summary>
        public static Single SlaveLerpStrength { get; set; } = 1f;

        /// <summary>
        /// The threshold that must be surpassed by the position
        /// in order for an instant slave snap to take place.
        /// </summary>
        public static Single PositionSnapThreshold { get; set; } = 5f;
        /// <summary>
        /// The threshold that must be surpassed by the rotation
        /// in order for an instant slave snap to take place.
        /// </summary>
        public static Single RotationSnapThreshold { get; set; } = MathHelper.PiOver2;
        #endregion

        #region Private Fields
        private Queue<FixtureContainer> _fixtures;
        private Category _collisionCategories;
        private Category _collidesWith;
        private Boolean _isSensor;
        #endregion

        #region Public Attributes
        public virtual BodyType BodyType
        {
            get => this.Get<BodyType>(b => b.BodyType);
            set
            {
                if(this.BodyType != value)
                {
                    this.Do(b => b.BodyType = value);
                    this.OnBodyTypeChanged?.Invoke(this, value);
                }
            }
        }
        public virtual Vector2 Position
        {
            get => this.Get<Vector2>(b => b.Position);
            set => this.SetTransformIgnoreContacts(value, this.master.Rotation);
        }
        public virtual Single Rotation
        {
            get => this.Get<Single>(b => b.Rotation);
            set => this.SetTransformIgnoreContacts(this.master.Position, value);
        }
        public virtual Vector2 LinearVelocity
        {
            get => this.Get<Vector2>(b => b.LinearVelocity);
            set => this.Do(b => b.LinearVelocity = value);
        }
        public virtual Single AngularVelocity
        {
            get => this.Get<Single>(b => b.AngularVelocity);
            set => this.Do(b => b.AngularVelocity = value);
        }
        public virtual Single LinearDamping
        {
            get => this.Get<Single>(b => b.LinearDamping);
            set => this.Do(b => b.LinearDamping = value);
        }
        public virtual Single AngularDamping
        {
            get => this.Get<Single>(b => b.AngularDamping);
            set => this.Do(b => b.AngularDamping = value);
        }
        public virtual Vector2 WorldCenter
        {
            get => this.Get<Vector2>(b => b.WorldCenter);
        }
        public virtual Vector2 LocalCenter
        {
            get => this.Get<Vector2>(b => b.LocalCenter);
            set => this.Do(b => b.LocalCenter = value);
        }
        public virtual Boolean Awake
        {
            get => this.Get<Boolean>(b => b.Awake);
            set => this.Do(b => b.Awake = value);
        }
        public virtual Boolean SleepingAllowed
        {
            get => this.Get<Boolean>(b => b.SleepingAllowed);
            set => this.Do(b => b.SleepingAllowed = value);
        }
        public IReadOnlyCollection<FixtureContainer> Fixtures
            => _fixtures;
        public Category CollisionCategories
        {
            get => _collisionCategories;
            set
            {
                if(value != _collisionCategories)
                {
                    _collisionCategories = value;
                    this.Do(b => b.SetCollisionCategories(value));
                    this.OnCollisionCategoriesChanged?.Invoke(this, this.CollisionCategories);
                }
            }
        }
        public Category CollidesWith
        {
            get => _collidesWith;
            set
            {
                if (value != _collisionCategories)
                {
                    _collidesWith = value;
                    this.Do(b => b.SetCollidesWith(value));
                    this.OnCollidesWithChanged?.Invoke(this, this.CollidesWith);
                }
            }
        }
        public Boolean IsSensor
        {
            get => _isSensor;
            set
            {
                if (value != _isSensor)
                {
                    _isSensor = value;
                    this.Do(b => b.SetIsSensor(value));
                    this.OnIsSensorChanged?.Invoke(this, this.IsSensor);
                }
            }
        }
        #endregion

        #region Events
        public event OnEventDelegate<BodyEntity, FixtureContainer> OnFixtureCreated;
        public event OnEventDelegate<BodyEntity, FixtureContainer> OnFixtureDestroyed;
        public event OnEventDelegate<BodyEntity, BodyType> OnBodyTypeChanged;
        public event OnEventDelegate<BodyEntity, Category> OnCollisionCategoriesChanged;
        public event OnEventDelegate<BodyEntity, Category> OnCollidesWithChanged;
        public event OnEventDelegate<BodyEntity, Boolean> OnIsSensorChanged;
        public event OnEventDelegate<BodyEntity> OnNudged;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.ValidateCleaning += this.HandleValidateCleaning;
            this.OnDo += this.HandleDo;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _fixtures = new Queue<FixtureContainer>();

            this.MessageHandlers[MessageType.Create].Add(this.CreateReadPosition, this.master.WritePosition);
        }

        protected override void Release()
        {
            base.Release();

            while (_fixtures.Any()) // Destroy all internal fixtures.
                _fixtures.Dequeue().Destroy();

            this.MessageHandlers[MessageType.Create].Remove(this.CreateReadPosition, this.master.WritePosition);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.ValidateCleaning += this.HandleValidateCleaning;
            this.OnDo -= this.HandleDo;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Run when the current entity should be "woken" up.
        /// This is automatically ran when <see cref="AetherEntity{T}.Do(Action{T})"/>
        /// is invoked. Practically, it can be used by the chunk to know when a <see cref="ShipParts.ShipPart"/>
        /// should be added back into quarantine.
        /// </summary>
        public void Nudge()
            => this.OnNudged?.Invoke(this);

        public virtual FixtureContainer BuildFixture(Shape shape)
            => this.BuildFixture(shape, this);
        public virtual FixtureContainer BuildFixture(Shape shape, BodyEntity owner)
            => new FixtureContainer(this, owner, shape).Then(fixture =>
            {
                _fixtures.Enqueue(fixture);

                this.Do(fixture.Attach);

                this.OnFixtureCreated?.Invoke(this, fixture);
            });
        public virtual void SetTransformIgnoreContacts(Vector2 position, Single angle)
            => this.Do(b => b.SetTransformIgnoreContacts(position, angle));

        public virtual void ApplyForce(Vector2 force, Vector2 point)
            => this.Do(b => b.ApplyForce(ref force, ref point));

        public virtual void ApplyForce(Vector2 force, Func<Body, Vector2> pointGetter)
            => this.Do(b => b.ApplyForce(force, pointGetter(b)));
        public virtual void ApplyForce(Func<Body, Vector2> forceGetter, Func<Body, Vector2> pointGetter)
            => this.Do(b =>
            {
                b.ApplyForce(forceGetter(b), pointGetter(b));
            });
        #endregion

        #region AetherChildEntity Implementation
        protected override Body Build(ServiceProvider provider, World parent)
            => parent.CreateBody();

        protected override AetherEntity<World> GetParent(ServiceProvider provider)
            => provider.GetService<WorldEntity>();

        protected override void Destroy()
            => this.Do(b => b.TryRemove());
        #endregion

        #region Event Handlers
        private bool HandleValidateCleaning(NetworkEntity sender, GameTime args)
            => this.live.FixtureList.Any();

        private void HandleDo(AetherEntity<Body> sender, Action<Body> args)
            => this.Nudge();
        #endregion

        #region Network Methods
        private void CreateReadPosition(NetIncomingMessage im)
        {
            this.master.ReadPosition(im);
            this.slave?.CopyPosition(this.master);
        }
        #endregion
    }
}
