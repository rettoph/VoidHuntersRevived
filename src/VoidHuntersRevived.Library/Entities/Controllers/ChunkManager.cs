using Guppy;
using Guppy.Lists;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO;
using log4net;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Collision;
using Guppy.Enums;
using Guppy.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    public class ChunkManager : Controller
    {
        #region Internal Classes
        internal class ChainChunks
        {
            private Chain _chain;

            /// <summary>
            /// A collection of all chunks that the 
            /// representing ship part belongs to.
            /// </summary>
            private HashSet<Chunk> _chunks;

            /// <summary>
            /// The last time the representing ShipPart was drawn.
            /// </summary>
            private Double _lastDraw;

            private ChunkManager _manager;

            public ChainChunks(ChunkManager manager, Chain chain)
            {
                _manager = manager;
                _chain = chain;
                _chunks = new HashSet<Chunk>();
                _lastDraw = default(Double);
            }

            internal void TryDraw(GameTime gameTime)
            {
                if(_lastDraw != gameTime.TotalGameTime.TotalMilliseconds)
                {
                    _chain.TryDraw(gameTime);
                    _lastDraw = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }

            /// <summary>
            /// Add the internal ship part into all of its chunks
            /// </summary>
            public void LoadChunks()
            {
                this.ClearChunks();
                Queue<ShipPart> children = new Queue<ShipPart>();
                children.Enqueue(_chain.Root);
                ShipPart target;
                Transform transform;
                AABB aabb;

                while (children.Any())
                {
                    // Select a singular child...
                    target = children.Dequeue();
                    target.live.GetTransform(out transform);

                    foreach (ShipPart child in target.Children)
                        children.Enqueue(child);

                    foreach (Fixture fixture in target.live.FixtureList)
                    { // Check all fixtures within each child...
                        fixture.Shape.ComputeAABB(out aabb, ref transform, 0);
    
                        var p = new Chunk.Position(aabb.Center);
                        if (_manager.chunks.ContainsKey(p) && !_chunks.Contains(_manager.chunks[p]))
                        {
                            _chunks.Add(_manager.chunks[p]);
                            _manager.chunks[p].Add(_chain);
                        }
                    }
                }
            }

            /// <summary>
            /// Remove the current ShipPart from all chunks.
            /// </summary>
            public void ClearChunks()
            {
                foreach (Chunk chunk in _chunks)
                    chunk.Remove(_chain);
                _chunks.Clear();
            }
        }
        #endregion

        #region Private Fields
        private ServiceProvider _provider;
        private WorldEntity _world;
        private EntityList _entities;
        private Dictionary<Chunk.Position, Chunk> _chunks;
        /// <summary>
        /// Creates a link between a chain and all of its
        /// current chunks.
        /// </summary>
        private Dictionary<Chain, ChainChunks> _chainChunks;

        /// <summary>
        /// New & incoming chain must pass quarantine
        /// before they can be added into a chunk.
        /// 
        /// (Essentially they just need to be asleep)
        /// </summary>
        private List<Chain> _quarantine;

        /// <summary>
        /// List of chains that have passed
        /// quarantine but are not yet added
        /// into a chunk.
        /// </summary>
        private Queue<Chain> _clean;
        #endregion

        #region Internal Fields
        internal Dictionary<Chain, ChainChunks> chainChunks => _chainChunks;
        internal WorldEntity world => _world;
        internal Dictionary<Chunk.Position, Chunk> chunks => _chunks;
        #endregion

        #region Public Properties
        public IReadOnlyCollection<Chain> Quarantine => _quarantine;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _chunks = new Dictionary<Chunk.Position, Chunk>();
            _chainChunks = new Dictionary<Chain, ChainChunks>();
            _quarantine = new List<Chain>();
            _clean = new Queue<Chain>();

            _provider = provider;
            provider.Service(out _entities);

            // Complete setup after world creation.
            provider.GetService<GameScene>().IfOrOnWorld(this.SetupWorld);

            this.LayerGroup = VHR.LayersContexts.Chunk.Group.GetValue();
        }

        protected override void Release()
        {
            base.Release();

            _provider = null;
            _entities = null;
            _world.OnSizeChanged -= this.HandleWorldSizeChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _quarantine.ForEach(sp => sp.TryDraw(gameTime));
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _quarantine.ForEach(chain =>
            {
                chain.TryUpdate(gameTime);

                if (chain.Root.LinearVelocity.Length() < 0.0001f)
                    chain.Root.LinearVelocity = Vector2.Zero;
                if (chain.Root.AngularVelocity < 0.0001f)
                    chain.Root.AngularVelocity = 0f;

                if (!chain.Root.Awake)
                    _clean.Enqueue(chain);
            });

            if (_clean.Any())
            {
                _world.live.Step(0);

                while (_clean.Any())
                    this.AddToChunks(_clean.Dequeue());
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add a ship part straight into all of its chunks.
        /// This assumes it has passed quarantine.
        /// </summary>
        /// <param name="chain"></param>
        private void AddToChunks(Chain chain)
        {
            // Remove the ship part from quarantine
            _quarantine.Remove(chain);

            if(chain != default && chain.Controller == this)
            { // Only proceed if the ShipPart is still a part of the ChunkManager...
                _chainChunks[chain].LoadChunks();

                chain.Root.OnNudged += this.HandleChainRootNudged;
            }
        }

        /// <summary>
        /// Save world info and auto call
        /// chunk builder.
        /// </summary>
        /// <param name="world"></param>
        private void SetupWorld(WorldEntity world)
        {
            _world = world;
            _world.OnSizeChanged += this.HandleWorldSizeChanged;
            this.BuildChunks();
        }

        /// <summary>
        /// Rebuild all world chunks.
        /// </summary>
        private void BuildChunks()
        {
            // Clear all pre-existing chunks...
            foreach (Chunk chunk in _chunks.Values)
                chunk.TryRelease();
            _chunks.Clear();

            // Create a new list of chunks based on the size of the world...
            for (Single x = 0; x < _world.Size.X; x += Chunk.Size)
            {
                for (Single y = 0; y < _world.Size.Y; y += Chunk.Size)
                {
                    var pos = new Chunk.Position(x, y);
                    _chunks.Add(pos, _provider.GetService<Chunk>((chunk, p, c) =>
                    {
                        chunk.SetPosition(pos);
                    }));
                }
            }

            // Re-add all internal entities
            if (this.chains.Any())
            {
                _world.live.Step(0);
                foreach(Chain chain in this.chains)
                {
                    _chainChunks[chain].ClearChunks();
                    if (!_quarantine.Contains(chain))
                        this.AddToChunks(chain);
                }
            }
        }

        /// <summary>
        /// Return all chunks visible within a recieved rectangle's
        /// view.
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public IEnumerable<Chunk> GetChunks(RectangleF bounds)
        {
            for (Single x = bounds.Left; x < bounds.Right; x += Chunk.Size)
            {
                for (Single y = bounds.Top; y < bounds.Bottom; y += Chunk.Size)
                {
                    var p = new Chunk.Position(x, y);
                    yield return _chunks[p];
                }
            }
        }
        #endregion

        #region Controller Methods
        /// <summary>
        /// Add an existing Chain into the
        /// Chunk manager.
        /// </summary>
        /// <param name="chain"></param>
        public new void TryAdd(Chain chain)
            => base.TryAdd(chain);

        protected override void Add(Chain chain)
        {
            base.Add(chain);

            chain.OnStatus[ServiceStatus.Releasing] += this.HandleChainReleasing;

            // Create a new ShipPartChunks instance.
            if (!_chainChunks.ContainsKey(chain))
                _chainChunks[chain] = new ChainChunks(this, chain);

            // Add the new ship part straight into quarantine
            _quarantine.Add(chain);

            // Update the new parts properties
            foreach(ShipPart shipPart in chain.Root.Items())
            {
                shipPart.SleepingAllowed = true;
                shipPart.Awake = true;

                // Update the new parts collisions
                shipPart.CollisionCategories = VHR.Categories.PassiveCollisionCategories;
                shipPart.CollidesWith = VHR.Categories.PassiveCollidesWith;
            }
        }

        protected override void Remove(Chain chain)
        {
            base.Remove(chain);

            // Remove the ship part from quarantine if needed...
            if (_quarantine.Contains(chain))
                _quarantine.Remove(chain);

            // Clear cached chunks
            _chainChunks[chain].ClearChunks();
            chain.Root.OnNudged -= this.HandleChainRootNudged;
            chain.OnStatus[ServiceStatus.Releasing] -= this.HandleChainReleasing;
        }
        #endregion

        #region Event Handlers
        private void HandleWorldSizeChanged(WorldEntity sender, Vector2 arg)
            => this.BuildChunks();


        /// <summary>
        /// When <see cref="BodyEntity.OnNudged"/> is called
        /// on an internal <see cref="ShipPart"/> we need to add it back
        /// into quarantine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="action"></param>
        private void HandleChainRootNudged(BodyEntity sender)
        {
            if(sender is ShipPart shipPart)
            {
                _chainChunks[shipPart.Chain].ClearChunks();
                _quarantine.Add(shipPart.Chain);

                shipPart.OnNudged -= this.HandleChainRootNudged;
            }
        }

        private void HandleChainReleasing(IService sender)
            => this.Remove(sender as Chain);
        #endregion
    }
}
