using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Farseer;
using FarseerPhysics.Collision.Shapes;
using Lidgren.Network;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    public class BodyEntity : FarseerChildEntity<Body, World>
    {
        #region Static Attributes
        /// <summary>
        /// The amount a slave body should lerp towards the master
        /// per millisecond
        /// </summary>
        public static Single SlaveLerpStrength { get; set; } = 0.0015625f;

        /// <summary>
        /// The threshold that must be surpassed by the position
        /// in order for an instant slave snap to take place.
        /// </summary>
        public static Single PositionSnapThreshold { get; set; } = 2f;
        /// <summary>
        /// The threshold that must be surpassed by the rotation
        /// in order for an instant slave snap to take place.
        /// </summary>
        public static Single RotationSnapThreshold { get; set; } = MathHelper.PiOver2;
        #endregion

        #region Private Fields
        private HashSet<FixtureContainer> _fixtures;
        private Category _collisionCategories;
        private Category _collidesWith;
        private Category _ignoreCCDWith;
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
                    this.Do(b => b.CollisionCategories = value);
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
                    this.Do(b => b.CollidesWith = value);
                    this.OnCollidesWithChanged?.Invoke(this, this.CollidesWith);
                }
            }
        }
        public Category IgnoreCCDWith
        {
            get => _ignoreCCDWith;
            set
            {
                if (value != _ignoreCCDWith)
                {
                    _ignoreCCDWith = value;
                    this.Do(b => b.IgnoreCCDWith = value);
                    this.OnIgnoreCCDWithChanged?.Invoke(this, this.IgnoreCCDWith);
                }
            }
        }
        #endregion

        #region Events
        public event GuppyEventHandler<BodyEntity, FixtureContainer> OnFixtureCreated;
        public event GuppyEventHandler<BodyEntity, FixtureContainer> OnFixtureDestroyed;
        public event GuppyEventHandler<BodyEntity, BodyType> OnBodyTypeChanged;
        public event GuppyEventHandler<BodyEntity, Category> OnCollisionCategoriesChanged;
        public event GuppyEventHandler<BodyEntity, Category> OnCollidesWithChanged;
        public event GuppyEventHandler<BodyEntity, Category> OnIgnoreCCDWithChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _fixtures = new HashSet<FixtureContainer>();
        }

        protected override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Helper Methods
        public virtual FixtureContainer BuildFixture(PolygonShape shape)
        {
            return this.BuildFixture(shape, this);
        }
        public virtual FixtureContainer BuildFixture(PolygonShape shape, BodyEntity owner)
        {
            var fixture = new FixtureContainer(this, owner, shape);
            _fixtures.Add(fixture);

            this.Do(fixture.Attach);

            this.OnFixtureCreated?.Invoke(this, fixture);

            return fixture;
        }

        public virtual Boolean DestroyFixture(FixtureContainer fixture)
        {
            if (_fixtures.Remove(fixture))
            {
                fixture.Destroy(true);
                this.OnFixtureDestroyed?.Invoke(this, fixture);
                return true;
            }

            return false;
        }

        public virtual void SetTransformIgnoreContacts(Vector2 position, Single angle)
        {
            if (this.Authorization == GameAuthorization.Partial)
                this.master.SetTransformIgnoreContacts(position, angle);
            else
                this.Do(b => b.SetTransformIgnoreContacts(position, angle));
        }

        public virtual void ApplyForce(Vector2 force, Vector2 point)
            => this.Do(b => b.ApplyForce(ref force, ref point));

        public virtual void ApplyForce(Vector2 force, Func<Body, Vector2> pointGetter)
            => this.Do(b => b.ApplyForce(force, pointGetter(b)));
        public virtual void ApplyForce(Func<Body, Vector2> forceGetter, Func<Body, Vector2> pointGetter)
            => this.Do(b => b.ApplyForce(forceGetter(b), pointGetter(b)));
        #endregion

        #region FarseerChildEntity Implementation
        protected override Body Build(ServiceProvider provider, World parent)
            => BodyFactory.CreateBody(parent);

        protected override FarseerEntity<World> GetParent(ServiceProvider provider)
            => provider.GetService<WorldEntity>();
        #endregion
    }
}
