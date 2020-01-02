using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Extensions.System.Collections;
using System.Linq;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// Represents a cell in the WorldScene. This will
    /// only update added  components once (on added)
    /// 
    /// This is for optimization reasons, as it allows
    /// for cheap FarseerEntitys when they are sitting 
    /// in the background.
    /// </summary>
    public sealed class Chunk : Controller
    {
        #region Static Properties
        public static Single Size { get; private set; } = 16;
        #endregion

        #region Private Fields
        private ChunkCollection _chunks;
        private IEnumerable<Chunk> _surrounding;
        #endregion

        #region Public Properties
        public ChunkPosition Position { get; internal set; }
        public RectangleF Bounds { get; private set; }
        #endregion

        #region Contructor
        public Chunk(ChunkCollection chunks)
        {
            _chunks = chunks;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            this.Bounds = new RectangleF(this.Position.X, this.Position.Y, Chunk.Size, Chunk.Size);

            base.Initialize();

            this.SetLayerDepth(1);

            this.OnAdded += this.HandleEntityAdded;
            this.OnRemoved += this.HandleEntityRemoved;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.OnAdded -= this.HandleEntityAdded;
            this.OnRemoved -= this.HandleEntityRemoved;
        }
        #endregion

        #region Setup Methods
        /// <inheritdoc />
        public override void SetupBody(FarseerEntity component, Body body)
        {
            base.SetupBody(component, body);

            // Stop all body movement
            body.SetTransformIgnoreContacts(component.Position, component.Rotation);
            body.LinearVelocity = Vector2.Zero;
            body.AngularVelocity = 0;
            body.CollisionCategories = Categories.PassiveCollisionCategories;
            body.CollidesWith = Categories.PassiveCollidesWith;
            body.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
            body.BodyType = BodyType.Dynamic;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get all chunks surrounding the current chunk.
        /// This does not include the current chunk
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chunk> GetSurrounding(Boolean create = true)
        {
            if (_surrounding == default(IEnumerable<Chunk>))
            {
                var list = new List<Chunk>();
                list.AddIfNotNull(_chunks.Get(this.Position.X + Chunk.Size, this.Position.Y + Chunk.Size, create));
                list.AddIfNotNull(_chunks.Get(this.Position.X + 0, this.Position.Y + Chunk.Size, create));
                list.AddIfNotNull(_chunks.Get(this.Position.X - Chunk.Size, this.Position.Y + Chunk.Size, create));

                list.AddIfNotNull(_chunks.Get(this.Position.X - Chunk.Size, this.Position.Y + 0, create));
                list.AddIfNotNull(_chunks.Get(this.Position.X + Chunk.Size, this.Position.Y + 0, create));

                list.AddIfNotNull(_chunks.Get(this.Position.X + Chunk.Size, this.Position.Y - Chunk.Size, create));
                list.AddIfNotNull(_chunks.Get(this.Position.X + 0, this.Position.Y - Chunk.Size, create));
                list.AddIfNotNull(_chunks.Get(this.Position.X - Chunk.Size, this.Position.Y - Chunk.Size, create));

                if (create || list.Count == 8)
                    _surrounding = list;
                else
                    return list;
            }

            return _surrounding;
        }
        #endregion

        #region Event Handlers
        private void HandleComponentDirtyChanged(object sender, bool arg)
        {
            if(arg)
            {
                this.Dirty = true;
                this.GetSurrounding().ForEach(c => c.Dirty = true);
            }
        }

        private void HandleEntityAdded(Object sender, FarseerEntity entity)
        {
            this.GetSurrounding().ForEach(c => c.Dirty = true);
            entity.OnDirtyChanged += this.HandleComponentDirtyChanged;
        }

        private void HandleEntityRemoved(Object sender, FarseerEntity entity)
        {
            this.GetSurrounding().ForEach(c => c.Dirty = true);
            entity.OnDirtyChanged -= this.HandleComponentDirtyChanged;
        }
        #endregion
    }
}
