using Guppy.DependencyInjection;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    public class AetherFixture : BaseAetherWrapper<Fixture>
    {
        #region Public Properties
        public AetherBody Body { get; internal set; }
        public Shape Shape { get; internal set; }

        /// <summary>
        /// Set the user data. Use this to store your application specific data.
        /// </summary>
        public Object Tag
        {
            set => this.Do(fixture => fixture.Tag = value);
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void Release()
        {
            base.Release();

            this.Do(fixture => fixture.TryRemove());
        }
        #endregion

        #region BaseAetherWrapper Implementation
        protected override Fixture BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
            => this.Body.Instances[authorization].CreateFixture(this.Shape);
        #endregion

        #region Helper Methods
        /// <summary>
        /// Update internal fixture data.
        /// </summary>
        /// <param name="collidesWith">The collision mask bits. This states the categories that this fixture would accept for collision.</param>
        /// <param name="collisionCategories">The collision categories this fixture is a part of.</param>
        /// <param name="collisionGroup">Collision groups allow a certain group of objects to never collide (negative) or always collide (positive). Zero means no collision group. Non-zero group filtering always wins against the mask bits.</param>
        public void SetCollisionData(Category collidesWith = Category.All, Category collisionCategories = Category.Cat1, Int16 collisionGroup = 0)
        {
            this.Do(fixture =>
            {
                fixture.CollidesWith = collidesWith;
                fixture.CollisionCategories = collisionCategories;
                fixture.CollisionGroup = 0;
            });
        }
        #endregion
    }
}
