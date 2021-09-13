using Guppy.DependencyInjection;
using Guppy.DependencyInjection.ServiceConfigurations;
using Guppy.Extensions.System;
using Guppy.Extensions.System.Collections;
using Guppy.Lists;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.Chunks;

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
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _bodies);
            provider.Service(out _chunks);

            this.BuildAetherInstances(provider);
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _spawnChunks = _chunks.GetChunks(Vector2.Zero, 1);
            _spawnChunks.ForEach(chunk => chunk.TryRegisterDependent(this.Id));
        }

        protected override void Release()
        {
            base.Release();

            _spawnChunks.ForEach(chunk => chunk.TryRegisterDependent(this.Id));
            _spawnChunks = default;
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            _chunks = default;

            _bodies.TryRelease();
            _bodies = default;
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
