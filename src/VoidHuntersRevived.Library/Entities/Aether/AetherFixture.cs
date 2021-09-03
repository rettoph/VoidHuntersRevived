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

        protected override Fixture BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
            => this.Body.Instances[authorization].CreateFixture(this.Shape);
    }
}
