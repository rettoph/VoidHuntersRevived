using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities
{
    /// <summary>
    /// An Entity that contains a body
    /// and can hold fixtures.
    /// </summary>
    public abstract class FarseerEntity : NetworkEntity
    {
        #region Protected Attributes
        protected Body body { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Build a new body for the entity. This only needs to be done once since the world is a scoped object.
            this.body = this.BuildBody(this.provider.GetRequiredService<World>());
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.body.Dispose();
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Generate the entity's body.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        protected abstract Body BuildBody(World world);
        #endregion
    }
}
