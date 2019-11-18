using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Extensions.Farseer;

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
        internal static Single Size { get; private set; } = 16;
        private static GameTime EmptyGameTime { get; set; } = new GameTime();
        #endregion

        #region Private Fields
        private ChunkCollection _chunks;
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
            base.Initialize();

            this.Bounds = new RectangleF(this.Position.X, this.Position.Y, Chunk.Size, Chunk.Size);
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
            body.BodyType = BodyType.Static;
        }
        #endregion

        #region Helper Methods
        public override bool Add(FarseerEntity entity)
        {
            if(this.Bounds.Contains(entity.Position.X, entity.Position.Y))
            { // If the entity resides within the current chunk...
                if (base.Add(entity))
                {
                    // Auto update the entity one last time
                    entity.TryUpdate(Chunk.EmptyGameTime);
                    return true;
                }
            }
            else
            { // If the entity does not reside in the current chunk...
                // Add the entity into its correct chunk...
                _chunks.Get(entity).Add(entity);
            }

            return false;
        }
        #endregion
    }
}
