using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.Chunks;
using Minnow.General;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    public class AetherWorld : BaseAetherWrapper<World>
    {
        #region Private Fields
        private FactoryServiceList<AetherBody> _bodies;
        private ChunkManager _chunks;
        private IEnumerable<Chunk> _spawnChunks;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _bodies);
            provider.Service(out _chunks);

            this.BuildAetherInstances(provider);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            // _spawnChunks = _chunks.GetChunks(Vector2.Zero, 10);
            // _spawnChunks.ForEach(chunk => chunk.TryRegisterDependent(this.Id));
        }

        protected override void PreUninitialize()
        {
            base.PreUninitialize();

            // _spawnChunks.ForEach(chunk => chunk.TryRegisterDependent(this.Id));
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            _bodies.Dispose();
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
        protected override World BuildInstance(ServiceProvider provider, NetworkAuthorization authorization)
            => new World(Vector2.Zero);
        #endregion

        #region CreateBody Methods
        /// <summary>
        /// Create a new AetherBody instance linked to this world.
        /// </summary>
        /// <returns></returns>
        public AetherBody CreateBody(Object tag)
        {
            return _bodies.Create((body, provider, _) =>
            {
                body.World = this;
                body.BuildAetherInstances(provider);

                body.Tag = tag;
            });
        }

        /// <summary>
        /// Create a new AetherBody instance linked to this world.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public AetherBody CreateBody(Action<AetherBody, ServiceProvider, ServiceConfiguration> setup)
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
