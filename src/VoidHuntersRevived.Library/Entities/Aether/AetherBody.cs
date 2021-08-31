using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    public class AetherBody : BaseAetherWrapper<Body>
    {
        #region Public Properties
        /// <summary>
        /// The owning <see cref="AetherWorld"/> instance. This will autmatically be defined
        /// on creation.
        /// </summary>
        public AetherWorld World { get; internal set; }

        /// <summary>
        /// Set the user data. Use this to store your application specific data.
        /// </summary>
        public Object Tag
        {
            set => this.Do(body => body.Tag = value);
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            if (this.World == default)
                throw new Exception("Improperly constructed AetherBody detected. Please ensure you call AetherWorld.CreateBody.");
        }

        protected override void PostRelease()
        {
            this.Do(body => body.TryRemove());

            base.PostRelease();
        }
        #endregion

        #region Helper Methods
        protected override Body BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
            => this.World.Instances[authorization].CreateBody().Then(body =>
            {
                body.CreateRectangle(1f, 1f, 0.5f, Vector2.Zero);
                body.LinearVelocity = Vector2.UnitX * 0.1f;
                body.BodyType = BodyType.Dynamic;
                body.LinearDamping = 0.5f;
            });

        /// <summary>
        /// Set the <see cref="Body.Tag"/> value for every internal
        /// <see cref="Body"/> instance.
        /// </summary>
        /// <param name="tag"></param>
        public void SetTag(Object tag)
            => this.Do(body => body.Tag = tag);
        #endregion
    }
}
