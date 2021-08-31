using Guppy.DependencyInjection;
using Guppy.DependencyInjection.ServiceConfigurations;
using Guppy.Lists;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    public class AetherWorld : BaseAetherWrapper<World>
    {
        #region Private Fields
        private FactoryServiceList<AetherBody> _bodies;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _bodies);

            this.BuildAetherInstances(provider);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Do(world => world.Step(gameTime.ElapsedGameTime));
        }
        #endregion

        #region Helper Methods
        protected override World BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
            => new World(Vector2.UnitY * 9.8f);
        #endregion

        #region CreateBody Methods
        /// <summary>
        /// Create a new AetherBody instance linked to this world.
        /// </summary>
        /// <returns></returns>
        public AetherBody CreateBody()
        {
            return _bodies.Create((body, provider, _) =>
            {
                body.World = this;
                body.BuildAetherInstances(provider);
            });
        }

        /// <summary>
        /// Create a new AetherBody instance linked to this world.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public AetherBody CreateBody(Action<AetherBody, GuppyServiceProvider, IServiceConfiguration> setup)
        {
            return _bodies.Create((body, provider, configuration) =>
            {
                body.World = this;
                body.BuildAetherInstances(provider);

                setup(body, provider, configuration);
            });
        }
        #endregion
    }
}
