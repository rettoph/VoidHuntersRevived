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
    public sealed class Chunk : SimpleController
    {
        #region Static Properties
        public static Single Size { get; private set; } = 16;
        public static GameTime EmptyGameTime { get; private set; } = new GameTime();
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
            body.BodyType = BodyType.Static;
        }
        #endregion

        #region Helper Methods
        protected override void TryClean(GameTime gameTime)
        {
            // Update all internal components
            this.Components.TryUpdateAll(Chunk.EmptyGameTime);

            // Continue the cleaning process...
            base.TryClean(gameTime);
        }

        public override bool Add(FarseerEntity entity)
        {
            if(this.Bounds.Contains(entity.Position.X, entity.Position.Y))
            { // If the entity resides within the current chunk...
                if(base.Add(entity))
                {
                    this.GetSurrounding().ForEach(c =>
                    {
                        if(c == null)
                        {
                            var test = this;
                        }
                        c.Dirty = true;
                    });
                    entity.Events.TryAdd<Boolean>("dirty:changed", this.HandleComponentDirtyChanged);
                    return true;
                }
            }
            else
            { // If the entity does not reside in the current chunk...
                // Add the entity into its correct chunk...
                _chunks.AddToChunk(entity);
            }

            return false;
        }

        public override bool Remove(FarseerEntity entity)
        {
            if(base.Remove(entity))
            {
                this.GetSurrounding().ForEach(c => c.Dirty = true);
                entity.Events.TryRemove<Boolean>("dirty:changed", this.HandleComponentDirtyChanged);

                return true;
            }

            return false;
        }

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
        #endregion
    }
}
