using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Structs;
using Guppy.Network;
using Guppy.Threading.Utilities;
using Guppy.Messages;

namespace VoidHuntersRevived.Library.Entities.Chunks
{
    public class Chunk : Frameable
    {
        #region Static Properties
        /// <summary>
        /// The constant size of a Chunk in Aether units.
        /// </summary>
        public static readonly Int32 Size = 64;

        /// <summary>
        /// The maximum amount of time a chunk can exist without any children before it will self-dispose.
        /// </summary>
        public static readonly Double MaxDependentlessThreshold = 500;
        #endregion

        #region Privage Fields
        private HashSet<Guid> _dependents;

        /// <summary>
        /// The amount of time in seconds the current chunk has gone without any dependents.
        /// </summary>
        private Double _dependentlessMilliseconds;

        private MessageBus _messageBus;
        #endregion

        #region Public Properties
        public Pipe Pipe { get; internal set; }
        public ChunkPosition Position { get; internal set; }
        public override Guid Id
        {
            get => this.Position.Id;
            protected set => throw new NotImplementedException();
        }

        public UInt16 Dependents { get; private set; }

        /// <summary>
        /// A list of all children linked to the current Chunk.
        /// </summary>
        public FrameableList<IWorldObject> Children { get; private set; }

        /// <summary>
        /// The chunk's current bounds.
        /// </summary>
        public Rectangle Bounds => new Rectangle(
            x: this.Position.X * Chunk.Size,
            y: this.Position.Y * Chunk.Size,
            width: Chunk.Size,
            height: Chunk.Size);
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _dependents = new HashSet<Guid>();

            this.Children = provider.GetService<FrameableList<IWorldObject>>();

            provider.Service(out _messageBus);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Dependents = 0;
            _dependentlessMilliseconds = 0;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            _dependents.Clear();
            this.Children.Dispose(true);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Children.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Children.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.Dependents == 0)
            {
                if (_dependentlessMilliseconds > Chunk.MaxDependentlessThreshold)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    _dependentlessMilliseconds += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            this.Children.TryUpdate(gameTime);
        }
        #endregion

        public Boolean TryRegisterDependent(Guid id)
        {
            if(_dependents.Add(id))
            {
                this.Dependents++;
                return true;
            }

            return false;
        }

        public Boolean TryDeregisterDependent(Guid id)
        {
            if (_dependents.Remove(id))
            {
                this.Dependents--;
                _dependentlessMilliseconds = 0;

                return true;
            }

            return false;
        }
    }
}
