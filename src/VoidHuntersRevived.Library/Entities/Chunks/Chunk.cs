using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;
using Guppy.Lists.Interfaces;
using Guppy.Network.Interfaces;
using Guppy.Network.Scenes;
using Guppy.Threading.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Structs;

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
        private ThreadQueue _updateThread;

        /// <summary>
        /// The amount of time in seconds the current chunk has gone without any dependents.
        /// </summary>
        private Double _dependentlessMilliseconds;
        #endregion

        #region Public Properties
        public IPipe Pipe { get; internal set; }
        public ChunkPosition Position { get; private set; }
        public override Guid Id
        {
            get => this.Position.Id;
            set
            {
                base.Id = value;
                this.Position = new ChunkPosition(value);
                this.Bounds = new Rectangle(
                    x: this.Position.X * Chunk.Size,
                    y: this.Position.Y * Chunk.Size,
                    width: Chunk.Size,
                    height: Chunk.Size);

                this.OnPositionSet?.Invoke(this, this.Position);
            }
        }

        public UInt16 Dependents { get; private set; }

        /// <summary>
        /// A list of all children linked to the current Chunk.
        /// </summary>
        public ServiceList<IWorldObject> Children { get; private set; }

        /// <summary>
        /// The chunk's current bounds.
        /// </summary>
        public Rectangle Bounds { get; private set;  }
        #endregion

        #region Events
        /// <summary>
        /// Internal helper method used to push the position set event to
        /// the <see cref="Components.Entities.Chunks.ChunkPipeComponent"/>.
        /// This will update the internal <see cref="Pipe"/> value when an id
        /// is defined.
        /// </summary>
        internal event OnEventDelegate<Chunk, ChunkPosition> OnPositionSet;

        /// <summary>
        /// Internal helper method invoked immediately after
        /// the <see cref="Children"/> value is set.
        /// 
        /// This is used by <see cref="Components.Entities.Chunks.ChunkPipeComponent"/>
        /// to setup required event listeners.
        /// </summary>
        internal event OnEventDelegate<Chunk, ServiceList<IWorldObject>> OnChildrenSet;
        #endregion

        #region Lifecycle Methods
        protected override void Create(GuppyServiceProvider provider)
        {
            base.Create(provider);

            _dependents = new HashSet<Guid>();
        }

        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Children = provider.GetService<ServiceList<IWorldObject>>();

            this.Children.OnAdded += this.HandleChildAdded;
            this.Children.OnRemoved += this.HandleChildRemoved;

            this.OnChildrenSet?.Invoke(this, this.Children);
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Dependents = 0;
            _dependentlessMilliseconds = 0;

            provider.Service(Guppy.Constants.ServiceConfigurationKeys.SceneUpdateThreadQueue, out _updateThread);
        }

        protected override void Release()
        {
            base.Release();

            _updateThread = default;

            _dependents.Clear();

            while (this.Children.Any())
            {
                this.Children.First().TryRelease();
            }
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            this.Children.OnAdded -= this.HandleChildAdded;
            this.Children.OnRemoved -= this.HandleChildRemoved;

            this.Children.TryRelease();
            // this.Children = default;
        }

        protected override void Dispose()
        {
            base.Dispose();

            _dependents = default;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.Dependents == 0)
            {
                if (_dependentlessMilliseconds > Chunk.MaxDependentlessThreshold)
                    _updateThread.Enqueue(gt => this.TryRelease());
                else
                    _dependentlessMilliseconds += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
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

        private void HandleChildAdded(IServiceList<IWorldObject> sender, IWorldObject worldObject)
        {
            // Remove the child from its old chunk...
            worldObject.Chunk?.Children.TryRemove(worldObject);

            // Update the entity's chunk...
            worldObject.Chunk = this;
        }

        private void HandleChildRemoved(IServiceList<IWorldObject> sender, IWorldObject worldObject)
        {
            // Update the entity's chunk...
            worldObject.Chunk = default;
        }
    }
}
