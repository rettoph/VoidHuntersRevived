using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
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

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    public class ChunkManager : Controller
    {
        #region Internal Ccasses
        internal class ShipPartChunks
        {
            private ShipPart _shipPart;

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

            public ShipPartChunks(ChunkManager manager, ShipPart shipPart)
            {
                _manager = manager;
                _shipPart = shipPart;
                _chunks = new HashSet<Chunk>();
                _lastDraw = default(Double);
            }

            internal void TryDraw(GameTime gameTime)
            {
                if(_lastDraw != gameTime.ElapsedGameTime.TotalMilliseconds)
                {
                    _shipPart.TryDraw(gameTime);
                    _lastDraw = gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            /// <summary>
            /// Add the internal ship part into all of its chunks
            /// </summary>
            public void LoadChunks()
            {
                this.ClearChunks();
                Queue<ShipPart> children = new Queue<ShipPart>();
                children.Enqueue(_shipPart);
                ShipPart child;
                Transform transform;
                AABB aabb;

                while (children.Any())
                {
                    // Select a singular child...
                    child = children.Dequeue();
                    child.live.GetTransform(out transform);
                    child.Children.ForEach(c => children.Enqueue(c));

                    foreach (Fixture fixture in child.live.FixtureList)
                    { // Check all fixtures within each child...
                        fixture.Shape.ComputeAABB(out aabb, ref transform, 0);

                        var p = new Chunk.Position(aabb.Center);
                        if (_manager.chunks.ContainsKey(p) && !_chunks.Contains(_manager.chunks[p]))
                        {
                            _chunks.Add(_manager.chunks[p]);
                            _manager.chunks[p].Add(_shipPart);
                        }
                    }
                }
            }

            /// <summary>
            /// Remove the current ShipPart from all chunks.
            /// </summary>
            public void ClearChunks()
            {
                _chunks.ForEach(c => c.Remove(_shipPart));
                _chunks.Clear();
            }
        }
        #endregion

        #region Private Fields
        private Logger _logger;
        private List<Chunk> _cache;
        private ServiceProvider _provider;
        private WorldEntity _world;
        private EntityCollection _entities;
        private Dictionary<Chunk.Position, Chunk> _chunks;
        /// <summary>
        /// Creates a link between a ShipPart and all of its
        /// current chunks.
        /// </summary>
        private Dictionary<ShipPart, ShipPartChunks> _shipPartChunks;

        /// <summary>
        /// New & incoming ShipParts must pass quarantine
        /// before they can be added into a chunk.
        /// 
        /// (Essentially they just need to be asleep)
        /// </summary>
        private List<ShipPart> _quarantine;

        /// <summary>
        /// List of ShipParts that have passed
        /// quarantine but are not yet added
        /// into a chunk.
        /// </summary>
        private Queue<ShipPart> _clean;
        #endregion

        #region Internal Fields
        internal Dictionary<ShipPart, ShipPartChunks> shipPartChunks => _shipPartChunks;
        internal WorldEntity world => _world;
        internal Dictionary<Chunk.Position, Chunk> chunks => _chunks;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _cache = new List<Chunk>();
            _chunks = new Dictionary<Chunk.Position, Chunk>();
            _shipPartChunks = new Dictionary<ShipPart, ShipPartChunks>();
            _quarantine = new List<ShipPart>();
            _clean = new Queue<ShipPart>();

            _provider = provider;
            provider.Service(out _entities);
            provider.Service(out _logger);

            // Complete setup after world creation.
            provider.GetService<GameScene>().IfOrOnWorld(this.SetupWorld);

            this.UpdateOrder = 110;
        }

        protected override void Dispose()
        {
            base.Dispose();

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

            _quarantine.ForEach(sp =>
            {
                sp.TryUpdate(gameTime);

                if(!sp.Awake)
                    _clean.Enqueue(sp);
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
        /// <param name="shipPart"></param>
        private void AddToChunks(ShipPart shipPart)
        {
            // Remove the ship part from quarantine
            _quarantine.Remove(shipPart);

            if(this.CanAdd(shipPart) && shipPart.Controller == this)
            { // Only proceed if the ShipPart is still a part of the ChunkManager...
                _shipPartChunks[shipPart].LoadChunks();
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
            _chunks.ForEach(c => c.Value.TryDispose());
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
            if (this.parts.Any())
            {
                _world.live.Step(0);
                this.parts.ForEach(p =>
                {
                    _shipPartChunks[p].ClearChunks();
                    if(!_quarantine.Contains(p))
                        this.AddToChunks(p);
                });
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
            _cache.Clear();
            for (Single x = bounds.Left; x < bounds.Right; x += Chunk.Size)
                for(Single y = bounds.Top; y < bounds.Bottom; y += Chunk.Size)
                {
                    var p = new Chunk.Position(x, y);
                    _cache.Add(_chunks[p]);
                }

            return _cache;
        }
        #endregion

        #region Controller Methods
        /// <summary>
        /// Add an existing ShipPart into the
        /// Chunk manager.
        /// </summary>
        /// <param name="shipPart"></param>
        public void TryAdd(ShipPart shipPart)
        {
            if (this.CanAdd(shipPart))
                this.Add(shipPart);
        }

        protected override void Add(ShipPart shipPart)
        {
            base.Add(shipPart);

            // Create a new ShipPartChunks instance.
            if (!_shipPartChunks.ContainsKey(shipPart)) 
                _shipPartChunks[shipPart] = new ShipPartChunks(this, shipPart);

            // Add the new ship part straight into quarantine
            shipPart.SleepingAllowed = true;
            shipPart.Awake = true;
            _quarantine.Add(shipPart);

            // Update the new parts collisions
            shipPart.CollisionCategories = Categories.PassiveCollisionCategories;
            shipPart.CollidesWith = Categories.PassiveCollidesWith;
            shipPart.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
        }

        protected override void Remove(ShipPart shipPart)
        {
            base.Remove(shipPart);

            // Remove the ship part from quarantine if needed...
            if (_quarantine.Contains(shipPart))
                _quarantine.Remove(shipPart);

            // Clear cached chunks
            _shipPartChunks[shipPart].ClearChunks();
        }
        #endregion

        #region Event Handlers
        private void HandleWorldSizeChanged(WorldEntity sender, Vector2 arg)
            => this.BuildChunks();
        #endregion
    }
}
