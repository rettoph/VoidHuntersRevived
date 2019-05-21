using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    public abstract class FarseerEntityDriver : Driver
    {
        protected FarseerEntity parent { get; private set; }

        public FarseerEntityDriver(
            FarseerEntity parent,
            EntityConfiguration configuration,
            Scene scene,
            ILogger logger) : base(configuration, scene, logger)
        {
            this.parent = parent;
        }

        #region Farseer Methods
        public virtual void ApplyLinearImpulse(Vector2 impulse)
        {
            this.parent.Body.ApplyLinearImpulse(impulse);
        }

        public virtual void ApplyAngularImpulse(Single impulse)
        {
            this.parent.Body.ApplyAngularImpulse(impulse);
        }

        public virtual void CreateFixture(Shape shape)
        {
            this.parent.Body.CreateFixture(shape, this.parent);
        }
        #endregion
    }
}
