using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Interfaces;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Entities.WorldObjects
{
    public abstract class WorldObject : MagicNetworkEntity, IWorldObject
    {
        #region Private Fields
        private Chunk _chunk;
        private Boolean _sleeping;
        private Boolean _dirty;
        #endregion

        #region Public Properties
        public abstract Vector2 Position { get; }
        public abstract Single Rotation { get; }

        public Chunk Chunk
        {
            get => _chunk;
        }
        Chunk IWorldObject.Chunk
        {
            get => _chunk;
            set => this.OnChunkChanged.InvokeIf(_chunk != value, this, ref _chunk, value);
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<IWorldObject, Chunk> OnChunkChanged;

        public event Step OnPreDraw;
        public event Step OnDraw;
        public event Step OnPostDraw;
        public event Step OnPreUpdate;
        public event Step OnUpdate;
        public event Step OnPostUpdate;
        #endregion

        #region Helper Methods
        public abstract void SetTransform(Vector2 position, Single rotation);
        #endregion

        #region Frame Methods
        public void TryDraw(GameTime gameTime)
        {
            this.OnPreDraw?.Invoke(gameTime);
            
            this.Draw(gameTime);

            this.OnDraw?.Invoke(gameTime);
            this.OnPostDraw?.Invoke(gameTime);
        }

        public void TryUpdate(GameTime gameTime)
        {
            this.OnPreUpdate?.Invoke(gameTime);

            this.Update(gameTime);

            this.OnUpdate?.Invoke(gameTime);
            this.OnPostUpdate?.Invoke(gameTime);
        }

        protected virtual void Draw(GameTime gameTime)
        {
            //
        }

        protected virtual void Update(GameTime gameTime)
        {
            //
        }
        #endregion


    }
}
